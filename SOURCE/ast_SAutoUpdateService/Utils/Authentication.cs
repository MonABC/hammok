using Hammock.AssetView.Platinum.Tools.Models;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;

namespace Hammock.AssetView.Platinum.Tools
{
    public class Authentication
    {
        internal static bool Authenticate(AuthenticateModel authenInfo)
        {
            if (authenInfo is null)
            {
                throw new ArgumentNullException(nameof(authenInfo));
            }

            if (authenInfo.Password is null)
            {
                throw new ArgumentNullException(nameof(authenInfo.Password));
            }

            string strPassword = Encrypt.DecryptToString(authenInfo.Password);

            if (authenInfo.UserName != AutoUpdateServiceSettings.API_USERNAME || strPassword != AutoUpdateServiceSettings.API_PASSWORD)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
