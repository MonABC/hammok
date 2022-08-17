using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    public static class BasicAuthHelpers
    {
        public static bool Parse(string header, out string name, out string password)
        {
            name = null;
            password = null;

            var headerValue = AuthenticationHeaderValue.Parse(header);

            // RFC 2617 sec 1.2, "scheme" name is case-insensitive
            if (headerValue.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) && headerValue.Parameter != null)
            {
                var credentials = Encoding.ASCII.GetString(Convert.FromBase64String(headerValue.Parameter));

                int separator = credentials.IndexOf(':');
                name = credentials.Substring(0, separator);
                password = credentials.Substring(separator + 1);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
