using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Client
{
    public class DeleteSessionRequestModel
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
    }
}
