using Newtonsoft.Json;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Client
{
    public class UserState
    {
        [JsonProperty("host_name")]
        public string HostName { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }
    }
}
