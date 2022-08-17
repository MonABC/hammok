using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common.Loggers;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Server;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Sessions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Controllers.V1.Server
{
    [Authorize]
    public class SessionsController : ApiController
    {
        public CreateNewSessionLoggerServer CreateNewSessionLoggerServer { get; set; } = new CreateNewSessionLoggerServer();
        public DeleteSessionsLogger DeleteSessionsLogger { get; set; } = new DeleteSessionsLogger("DeleteSessions_Server");
        private SessionsManager GetSessionsManager()
        {
            var sessionsManager = HttpContext.Current.Application["Sessions"] as SessionsManager;

            if (sessionsManager == null)
            {
                Logger.Error("Sessionsのインスタンスが存在しないエラー。");

                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError));
            }

            return sessionsManager;
        }

        private string GetCompanyId()
        {
            string companyId, password;

            if (!BasicAuthHelpers.Parse(HttpContext.Current.Request.Headers["Authorization"], out companyId, out password))
            {
                Logger.Error("Basic認証解析エラー。");

                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError));
            }

            return companyId;
        }

        // 新しいセッションの作成。
        [HttpPost]
        public CreateSessionResultModel Post(CreateSessionRequestModel parameter)
        {
            try
            {
                var companyId = this.GetCompanyId();
                var sessionsManager = this.GetSessionsManager();

                var sessionInfo = sessionsManager.CreateSession(companyId, parameter.HostName, parameter.UserName);
                if (sessionInfo == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.ServiceUnavailable));

                var createdSession = new CreateSessionResultModel
                {
                    SessionId = sessionInfo.SessionId,
                    RelayServiceHostName = ConfigurationManager.AppSettings["HostName"],
                    ServerListenPort = sessionInfo.Server.ListenPort,
                    ExpiredTime = sessionInfo.ExpiredTime,
                };
                CreateNewSessionLoggerServer.Write(companyId, parameter, createdSession);
                return createdSession;
            }
            catch (TooManySessionsException)
            {
                throw new HttpResponseException(Request.CreateResponse((HttpStatusCode)429));
            }
        }

        // セッションの削除。
        [HttpDelete]
        public void Delete([FromUri] string id)
        {
            var companyId = this.GetCompanyId();
            var sessionsManager = this.GetSessionsManager();

            sessionsManager.DeleteSession(companyId, id);
            DeleteSessionsLogger.Write(companyId, id);
        }
    }
}
