using Hammock.AssetView.Platinum.Tools.RC.RelayClient.DTOs;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels
{
    public class LoginWindowViewModel : BindableBase, IClosable
    {
        private readonly IDialogService dialogService;
        private const string _configFilePath = "astRC_RelayClient_LoginSetting.esf";

        public ILoginWindowViewModelValidator Validator { get; set; } = new LoginWindowViewModelValidator
        {
            CloudUrl = ConfigurationManager.AppSettings["CloudUrl"],
        };
        public IMessageDialog MessageDialog { get; set; }

        public Action CloseEvent;

        private LoginSetting _setting;

        private string companyId;
        public string CompanyId
        {
            get { return companyId; }
            set
            {
                companyId = value;
                RaisePropertyChanged();
            }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                RaisePropertyChanged();
            }
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

        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public LoginWindowViewModel()
        {
            OkCommand = new DelegateCommand(async () => { await OK(); });
            CancelCommand = new DelegateCommand(Cancel);
            LoadedCommand = new DelegateCommand(async () => { await Loaded(); });
        }

        public LoginWindowViewModel(IDialogService ds)
            : this()
        {
            dialogService = ds ?? throw new ArgumentNullException(nameof(ds));
        }

        public ICommand LoadedCommand { get; set; }
        private async Task Loaded()
        {
            bool executable = false;
            await Task.Run(() => { executable = LoadedInternal(); });
            IsEnabled = true;

#pragma warning disable CA1508 // Avoid dead conditional code
            if (executable == false)
            {
                MessageDialog.Warn(RelayClient.Properties.Resources.NotActivated);
                this.Close();
            }
#pragma warning restore CA1508 // Avoid dead conditional code
        }

        private bool LoadedInternal()
        {
            // 動作環境を検証する。
            if (ExecutablePC() == false)
            {
                return false;
            }

            // ログイン設定の読み込み。
            if (File.Exists(_configFilePath))
            {
                _setting = SettingsFramework.Load<LoginSetting>(_configFilePath);
            }
            else
            {
                _setting = new LoginSetting();
            }

            CompanyId = _setting.CompanyId;
            Password = _setting.Password;
            return true;
        }

        private bool ExecutablePC()
        {
            return RcUtils.GetAssetViewClientActivationState();
        }

        private async Task OK()
        {
            IsEnabled = false;
            try
            {
                var message = await Task.Run<string>(() =>
                {
                    return Validator.Validate(this);
                });

                if (string.IsNullOrEmpty(message) == false)
                {
                    MessageDialog.Warn(message);
                    return;
                }

                _setting.CompanyId = CompanyId;
                _setting.Password = Password;
                SettingsFramework.Save(_configFilePath, _setting);

                var parameter = new DialogParameters
                {
                    { nameof(LoginSetting), _setting },
                    { "Owner", this }
                };
                dialogService.Show(
                    nameof(SwitchWindowViewModel),
                    parameter,
                    r =>
                    {
                    });
            }
            finally
            {
                IsEnabled = true;
            }
        }

        private void Cancel()
        {
            Close();
        }

        public void Close()
        {
            CloseEvent();
        }
    }
}
