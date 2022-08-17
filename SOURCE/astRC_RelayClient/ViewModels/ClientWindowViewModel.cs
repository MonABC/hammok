using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Common;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.DTOs;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.WebApi;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Properties;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Client;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels
{
    public class ClientWindowViewModel : BindableBase, IDialogAware
    {
        public IProgressDialogShower ProgressDialogShower { get; set; }
        public IMessageDialog MessageDialog { get; set; }

        private ClientWebApi _webApi;
        private RcClientManager _rcClientManager;
        private RccManager _rccManager;

        public event Action<IDialogResult> RequestClose;

        private string _state;
        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
                RaisePropertyChanged();
            }
        }

        private string _sessionId = "";
        public string SessionId
        {
            get { return _sessionId; }
            set
            {
                _sessionId = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CanClickConnectButton));
            }
        }

        private readonly Regex regex = new Regex(@"^\d{4}$", RegexOptions.Compiled);
        public bool CanClickConnectButton
        {
            get
            {
                return regex.IsMatch(SessionId);
            }
        }


        public ListCollectionView ListViewContents { get; private set; }
        private readonly ObservableCollectionEx<ClientListViewItem> _listViewModels = new ObservableCollectionEx<ClientListViewItem>();

        public ClientListViewItem _selectedItem;
        public ClientListViewItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CanClickKillButton));
            }
        }

        public bool CanClickKillButton
        {
            get
            {
                return SelectedItem != null;
            }
        }

        public ICommand ItemClickCommand { get; private set; }

        public ICommand ConnectCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand KillCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                RaisePropertyChanged();
            }
        }

        public string Title => Properties.Resources.ClientWindow_WindowTitle;

        private readonly Dictionary<string, Task> _tasks = new Dictionary<string, Task>();

        public ClientWindowViewModel()
        {
            ListViewContents = (ListCollectionView)CollectionViewSource.GetDefaultView(_listViewModels);
            ListViewContents.Filter += (_) => true;

            ItemClickCommand = new DelegateCommand<ClientListViewItem>(SelectedItemChanged);
            ConnectCommand = new DelegateCommand(Connect);
            RefreshCommand = new DelegateCommand(() => { _ = Refresh(); });
            KillCommand = new DelegateCommand(Kill);
            CloseCommand = new DelegateCommand(Close);
            LoadedCommand = new DelegateCommand(async () => { await Loaded(); });
        }

        public ICommand LoadedCommand { get; set; }
        private async Task Loaded()
        {
            await Task.Run(() => { LoadedInternal(); });
            IsEnabled = true;
        }

        private void LoadedInternal()
        {
            _ = Refresh(true);
        }

        private void SelectedItemChanged(ClientListViewItem item)
        {
            if (item == null)
            {
                return;
            }

            SessionId = item.SessionId;
        }

        internal void End()
        {
            Action action = () =>
            {
                Task.WaitAll(_tasks.Values.ToArray());
            };

            _rccManager.RelayEnd();
            ProgressDialogShower.ShowProgressWindowDialog(action, Resources.ClientWindow_WaitingForEndOfRemoteConnection);
        }

        private void Connect()
        {
            var model = _listViewModels.FirstOrDefault(n => n.SessionId == SessionId);

            if (model == null)
            {
                MessageDialog.Warn(Resources.ClientWindow_ConnectionIdDoesNotExists);
                return;
            }
            if (model.ExpiredTime < DateTime.Now)
            {
                MessageDialog.Warn(string.Format(Resources.ClientWindow_TheConnectionIdHasExpired, model.SessionId));
                return;
            }
            if (_tasks.ContainsKey(model.SessionId))
            {
                MessageDialog.Warn(string.Format(Resources.ClientWindow_AlreadyConnected, model.SessionId));
                return;
            }

            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    _rcClientManager.Run(model.SessionId);
                }
                catch (WebException e)
                {
                    Logger.Error(e);
                    MessageDialog.Warn(Resources.UnableToConnectToTheCloud);
                }
                catch (RCRelayClientException e)
                {
                    Logger.Error(e);
                    MessageDialog.Warn(e.Message);
                }
            }).ContinueWith(t =>
            {
                if (t.Exception == null)
                {
                    _ = _tasks.Remove(model.SessionId);
                }
            });
            _tasks.Add(model.SessionId, task);
        }

        private bool Refresh(bool rethrow = false)
        {
            bool flag = false;

            Action action = () =>
            {
                try
                {
                    _listViewModels.Clear();

                    int totalCount = 0;
                    int connectedCount = 0;

                    foreach (var info in _webApi.GetSessionList())
                    {
                        var item = new ClientListViewItem()
                        {
                            SessionId = info.SessionId,
                            SessionState = info.SessionState == SessionState.Connected ? Resources.ClientWindow_Connecting : Resources.ClientWindow_Waiting,
                            ExpiredTime = info.ExpiredTime.ToLocalTime(),
                            RcClientHostName = info.ClientState.HostName,
                            RcClientUserName = info.ClientState.UserName,
                            RcServerHostName = info.ServerState.HostName,
                            RcServerUserName = info.ServerState.UserName,
                        };

                        totalCount++;
                        if (info.SessionState == SessionState.Connected)
                        {
                            connectedCount++;
                        }

                        _listViewModels.Add(item);
                    }

                    App.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        State = string.Format("{0}/{1}", connectedCount, totalCount);
                    }));

                    flag = true;
                }
                catch (WebException e)
                {
                    Logger.Error(e);
                    if (rethrow)
                    {
                        throw;
                    }
                    else
                    {
                        MessageDialog.Warn(Resources.UnableToConnectToTheCloud);
                    }
                }
            };

            ProgressDialogShower.ShowProgressWindowDialog(action, Resources.GettingConnectionList);

            return flag;
        }

        private void Kill()
        {
            var id = SelectedItem.SessionId;

            if (MessageDialog.Confirm(string.Format(Resources.ClientWindow_Are_you_sure_you_want_to_close, id)) == false)
            {
                return;
            }

            Action action = () =>
            {
                try
                {
                    _webApi.DeleteSessions(new[] { new DeleteSessionRequestModel() { SessionId = id } });
                }
                catch (WebException e)
                {
                    Logger.Error(e);
                    MessageDialog.Warn(Resources.UnableToConnectToTheCloud);
                }
            };

            ProgressDialogShower.ShowProgressWindowDialog(action, Resources.ClientWindow_DeletingConnection);

            _ = Refresh();
        }

        private void Close()
        {
            RequestClose(new DialogResult());
        }

        public bool CanCloseDialog()
        {
            End();
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var cloudUrl = ConfigurationManager.AppSettings["CloudUrl"];
            LoginSetting loginConfig = parameters.GetValue<LoginSetting>(nameof(LoginSetting));

            _webApi = new ClientWebApi();
            _webApi.CloudUrl = cloudUrl;
            _webApi.UserName = loginConfig.CompanyId;
            _webApi.Password = loginConfig.Password;
            _rcClientManager = new RcClientManager(_webApi);
            _rccManager = new RccManager();
        }
    }
}
