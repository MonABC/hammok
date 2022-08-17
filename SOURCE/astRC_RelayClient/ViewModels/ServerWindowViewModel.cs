using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Common;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.DTOs;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.WebApi;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Properties;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels
{
    public class ServerWindowViewModel : BindableBase, IDialogAware
    {
        private bool tooManySessions = false;
        public IProgressDialogShower ProgressDialogShower { get; set; }

        private ServerWebApi _webApi;
        private RcServerManager _rcServerManager;
        public IMessageDialog MessageDialog { get; set; }

        public event Action<IDialogResult> RequestClose;
        public RccManager RccManager { get; set; } = new RccManager();

        private string _expiredTimeString;
        public string ExpiredTimeString
        {
            get { return _expiredTimeString; }
            set
            {
                _expiredTimeString = value;
                RaisePropertyChanged();
            }
        }

        private string _sessionId;
        public string SessionId
        {
            get { return _sessionId; }
            set
            {
                _sessionId = value;
                RaisePropertyChanged();
            }
        }

        public ICommand CloseCommand { get; private set; }

        public string Title => Properties.Resources.ServerWindow_WindowTitle;

        public ServerWindowViewModel()
        {
            CloseCommand = new DelegateCommand(Close);
            LoadedCommand = new DelegateCommand(async () => { await Loaded(); });
        }

        public ICommand LoadedCommand { get; set; }
        private async Task Loaded()
        {
            await Task.Run(() =>
            {
                try
                {
                    Start();
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse)e.Response).StatusCode == (HttpStatusCode)429)
                    {
                        tooManySessions = true;
                        MessageDialog.Warn(Resources.TooManySessions);
                        Dispatcher.Invoke(() =>
                        {
                            RequestClose(new DialogResult(ButtonResult.Cancel));
                        });
                    }
                    else
                    {
                        throw;
                    }
                }
            });

            IsEnabled = true;
        }

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

        public Dispatcher Dispatcher { get; internal set; }

        private void Start()
        {
            Action action = () =>
            {
                RccManager.InitiateCommunicationWithAstRCC();

                var result = _rcServerManager.Start();

                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ExpiredTimeString =
                    String.Format(Resources.ServerWindow_ConnectionIdIsValidUntil, result.ExpiredTime.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"));
                    SessionId = result.SessionId;
                }));
            };

            ProgressDialogShower.ShowProgressWindowDialog(action, Resources.WaitingForRemoteConnection);
        }

        private void End()
        {
            Action action = () =>
            {
                try
                {
                    _rcServerManager.Stop();
                }
                catch (WebException e)
                {
                    Logger.Error(e);
                    MessageDialog.Warn(Properties.Resources.UnableToConnectToTheCloud);
                }
            };

            ProgressDialogShower.ShowProgressWindowDialog(action, Resources.ProgressWindow_WaitingForRemoteConnection);
        }

        private void Close()
        {
            RequestClose(new DialogResult(ButtonResult.OK));
        }

        public bool CanCloseDialog()
        {
            if (tooManySessions == false)
            {
                if (MessageDialog.Confirm(Properties.Resources.ServerWindow_AreYouOkToQuitWaiting) == false)
                {
                    return false;
                }
            }

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

            _webApi = new ServerWebApi(cloudUrl, loginConfig.CompanyId, loginConfig.Password);
            _rcServerManager = new RcServerManager(_webApi);
        }
    }
}
