using Hammock.AssetView.Platinum.Tools.RC.RelayClient.DTOs;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels
{
    public class SwitchWindowViewModel : IDialogAware
    {
        private IClosable _owner;
        private readonly IDialogService dialogService;
        private LoginSetting loginSetting;

#pragma warning disable CS0067
        public event Action<IDialogResult> RequestClose;
#pragma warning restore CS0067

        public ICommand ClientModeCommand { get; private set; }
        public ICommand ServerModeCommand { get; private set; }

        public string Title => Properties.Resources.SelectModeWindowTitle;

        public SwitchWindowViewModel()
        {
            ClientModeCommand = new DelegateCommand(ClientMode);
            ServerModeCommand = new DelegateCommand(ServerMode);
            LoadedCommand = new DelegateCommand(Loaded);
        }

        public SwitchWindowViewModel(IDialogService s)
            : this()
        {
            dialogService = s ?? throw new ArgumentNullException(nameof(s));
        }

        public ICommand LoadedCommand { get; set; }
        private void Loaded()
        {
            _owner.Close();
        }

        private void ClientMode()
        {
            var parameter = new DialogParameters
            {
                { nameof(LoginSetting), loginSetting }
            };
            dialogService.Show(
                nameof(ClientWindowViewModel),
                parameter,
                r =>
                {
                });
        }

        public void ServerMode()
        {
            var parameter = new DialogParameters
            {
                { nameof(LoginSetting), loginSetting }
            };
            dialogService.Show(
                nameof(ServerWindowViewModel),
                parameter,
                r =>
                {
                });
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            loginSetting = parameters.GetValue<LoginSetting>(nameof(LoginSetting));
            _owner = parameters.GetValue<IClosable>("Owner");
        }
    }
}

