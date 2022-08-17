using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Server
{
    public class CreateSessionRequestModel
    {
        [JsonProperty("host_name")]
        public string HostName { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }
    }
}
