using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.WebApi;
using System;
using System.Net;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels
{
    public class LoginWindowViewModelValidator : ILoginWindowViewModelValidator
    {
        public IClientWebApi ClientWebApi { get; set; } = new ClientWebApi();
        public string CloudUrl { get; set; }

        public string Validate(LoginWindowViewModel vm)
        {
            if (vm == null)
            {
                throw new ArgumentNullException(nameof(vm));
            }

            if (string.IsNullOrWhiteSpace(vm.CompanyId))
            {
                return Properties.Resources.LoginWindowViewModelValidator_PleaseInputCompanyId;
            }
            if (string.IsNullOrWhiteSpace(vm.Password))
            {
                return Properties.Resources.LoginWindowViewModelValidator_PleaseInputPassword;
            }

            ClientWebApi.CloudUrl = CloudUrl;
            ClientWebApi.UserName = vm.CompanyId;
            ClientWebApi.Password = vm.Password;

            try
            {
                _ = ClientWebApi.GetSessionList();
                return string.Empty;
            }
            catch (WebException)
            {
                // GetSessionListメソッド内で、ログ出力しているため、このメソッドではログ出力不要。
                return Properties.Resources.LoginWindowViewModelValidator_CanNotConnectRelayServer;
            }
        }
    }
}