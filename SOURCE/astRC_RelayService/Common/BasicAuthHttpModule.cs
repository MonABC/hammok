using Hammock.AssetView.Platinum.Tools.RC.RelayService.Sessions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    // https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/basic-authentication
    public class BasicAuthHttpModule : IHttpModule
    {
        private const string Realm = "Secret Zone";

        // TODO: Here is where you would validate the username and password.
        private bool CheckPassword(string username, string password)
        {
            var sessionsManager = HttpContext.Current.Application["Sessions"] as SessionsManager;
            if (sessionsManager == null) return false;

            return sessionsManager.Authentication(username, password);
        }

        public void Init(HttpApplication context)
        {
            // Register event handlers
            context.AuthenticateRequest += this.OnApplicationAuthenticateRequest;
            context.EndRequest += this.OnApplicationEndRequest;
        }

        private void OnApplicationAuthenticateRequest(object sender, EventArgs e)
        {
            var request = HttpContext.Current.Request;
            var authHeader = request.Headers["Authorization"];
            if (authHeader != null)
            {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                if (authHeaderVal.Scheme.Equals("basic",
                        StringComparison.OrdinalIgnoreCase) &&
                    authHeaderVal.Parameter != null)
                {
                    this.AuthenticateUser(authHeaderVal.Parameter);
                }
            }
        }

        // If the request was unauthorized, add the WWW-Authenticate header
        // to the response.
        private void OnApplicationEndRequest(object sender, EventArgs e)
        {
            var response = HttpContext.Current.Response;
            if (response.StatusCode == 401)
            {
                response.Headers.Add("WWW-Authenticate",
                    string.Format("Basic realm=\"{0}\"", Realm));
            }
        }

        private void AuthenticateUser(string credentials)
        {
            try
            {
                credentials = Encoding.ASCII.GetString(Convert.FromBase64String(credentials));

                int separator = credentials.IndexOf(':');
                string name = credentials.Substring(0, separator);
                string password = credentials.Substring(separator + 1);

                if (CheckPassword(name, password))
                {
                    var identity = new GenericIdentity(name);
                    this.SetPrincipal(new GenericPrincipal(new GenericIdentity("user"), new[] { "IIS_IUSRS" }));
                }
                else
                {
                    // Invalid username or password.
                    HttpContext.Current.Response.StatusCode = 401;
                }
            }
            catch (FormatException)
            {
                // Credentials were not formatted correctly.
                HttpContext.Current.Response.StatusCode = 401;
            }
        }

        private void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;

            if (HttpContext.Current.Items != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        public void Dispose()
        {

        }
    }
}
