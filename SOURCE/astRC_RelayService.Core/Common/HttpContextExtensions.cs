using System;
using System.Web;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    internal static class HttpContextExtensions
    {
        internal static string GetClientIP(this HttpContext httpContext)
        {
            if (httpContext is null) { throw new ArgumentNullException(nameof(httpContext)); }

            var clientIp = "";
            var xForwardedFor = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (String.IsNullOrEmpty(xForwardedFor) == false)
            {
                clientIp = xForwardedFor.Split(',').GetValue(0).ToString().Trim();
            }
            else
            {
                clientIp = httpContext.Request.UserHostAddress;
            }

            if (clientIp != "::1"/*localhost*/)
            {
                clientIp = clientIp.Split(':').GetValue(0).ToString().Trim();
            }

            return clientIp;
        }
    }
}

