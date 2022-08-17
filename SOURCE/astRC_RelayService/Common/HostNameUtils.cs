using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    public static class HostNameUtils
    {
        public static string GetIpAddress(string hostName)
        {
            IPAddress ipAddress;
            if (IPAddress.TryParse(hostName, out ipAddress)) return ipAddress.ToString();

            return System.Net.Dns.GetHostEntry(hostName).AddressList.Select(n => n.ToString()).FirstOrDefault();
        }
    }
}
