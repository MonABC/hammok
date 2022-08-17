using Newtonsoft.Json;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.DTOs
{
    public class LoginSetting
    {
        [JsonProperty("company_id")]
        public string CompanyId { get; set; }

        [JsonProperty]
        public string Password { get; set; }
    }
}


