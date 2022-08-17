using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Client
{
    public class GetSessionResultModel
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("relay_service_host_name")]
        public string RelayServiceHostName { get; set; }

        [JsonProperty("client_listen_port")]
        public int ClientListenPort { get; set; }

        [JsonProperty("creation_time")]
        public DateTime CreationTime { get; set; }

        [JsonProperty("expired_time")]
        public DateTime ExpiredTime { get; set; }

        [JsonProperty("session_state")]
        public SessionState SessionState { get; set; }

        [JsonProperty("client_state")]
        public UserState ClientState { get; set; }

        [JsonProperty("server_state")]
        public UserState ServerState { get; set; }
    }
}

