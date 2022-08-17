using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Server
{
    public class CreateSessionResultModel
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("relay_service_host_name")]
        public string RelayServiceHostName { get; set; }

        [JsonProperty("server_listen_port")]
        public int ServerListenPort { get; set; }

        [JsonProperty("expired_time")]
        public DateTime ExpiredTime { get; set; }
    }
}

