using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Models
{
    [JsonObject]
    public class SessionInfoModel
    {
        [JsonConstructor]
        public SessionInfoModel(
            string companyId, string sessionId, string ip, int processId,
            DateTime creationTime, DateTime updateTime, DateTime expiredTime,
            SessionState state, UserInfoModel client, UserInfoModel server)
        {
            this.CompanyId = companyId;
            this.SessionId = sessionId;
            this.ProcessId = processId;
            this.IpAddress = ip;
            this.CreationTime = creationTime;
            this.UpdateTime = updateTime;
            this.ExpiredTime = expiredTime;
            this.State = state;
            this.Client = client;
            this.Server = server;
        }

        [JsonProperty("company_id")]
        public string CompanyId { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("process_id")]
        public int ProcessId { get; set; }

        [JsonProperty("creation_time")]
        public DateTime CreationTime { get; set; }

        [JsonProperty("update_time")]
        public DateTime UpdateTime { get; set; }

        [JsonProperty("expired_time")]
        public DateTime ExpiredTime { get; set; }

        [JsonProperty("state")]
        public SessionState State { get; set; }

        [JsonProperty("client")]
        public UserInfoModel Client { get; set; }

        [JsonProperty("server")]
        public UserInfoModel Server { get; set; }

        [JsonProperty("ip_address")]
        public string IpAddress { get; set; }
    }
}
