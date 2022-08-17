using Newtonsoft.Json;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Models
{
    [JsonObject]
    public class UserInfoModel
    {
        [JsonConstructor]
        public UserInfoModel(int listenPort, string hostName, string userName)
        {
            this.ListenPort = listenPort;
            this.HostName = hostName;
            this.UserName = userName;
        }

        [JsonProperty("listen_port")]
        public int ListenPort { get; set; }

        [JsonProperty("host_name")]
        public string HostName { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }
    }
}
