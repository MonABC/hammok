using Hammock.AssetView.Platinum.Tools.RC.RelayService.AppStart;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Sessions;
using System;
using System.Configuration;
using System.Web;
using System.Web.Http;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            WebApiConfig.Register(GlobalConfiguration.Configuration);

            Logger.Debug("SessionsManager Start");
            // SessionsManager Start
            {
                _ = int.TryParse(ConfigurationManager.AppSettings["MaxSessions"], out int maxSessions);
                Logger.Debug($"MaxSessions={maxSessions}");
                Logger.Debug($"HttpContext.Current.Server.MapPath={HttpContext.Current.Server.MapPath("./ ")}");
                SessionsManager sessions = null;
                try
                {
                    sessions = new SessionsManager(HttpContext.Current.Server.MapPath("./"), maxSessions);
                    sessions.Init();

                    this.Application.Set("Sessions", sessions);
                }
                catch (Exception)
                {
                    sessions?.Dispose();
                    throw;
                }
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error((Exception)e.ExceptionObject);
        }

        protected void Application_End()
        {
            Logger.Debug("SessionsManager End");
            // SessionsManager End
            {
                var sessions = this.Application.Get("Sessions") as SessionsManager;

                if (sessions != null)
                {
                    sessions.Save();
                    sessions.Dispose();
                }
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }
    }
}
