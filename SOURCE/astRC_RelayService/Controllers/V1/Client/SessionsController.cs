using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common.Loggers;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Client;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Sessions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Controllers.V1.Client
{
    [Authorize]
    public class SessionsController : ApiController
    {
        public GetSessionsLoggerClient GetSessionsLoggerClient { get; set; } = new GetSessionsLoggerClient();
        public UpdateSessionsLoggerClient UpdateSessionsLoggerClient { get; set; } = new UpdateSessionsLoggerClient();
        public DeleteSessionsLogger DeleteSessionsLogger { get; set; } = new DeleteSessionsLogger("DeleteSessions_Client");
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

        // セッションリストの取得。
        [HttpGet]
        public IEnumerable<GetSessionResultModel> Get()
        {
            var companyId = this.GetCompanyId();
            GetSessionsLoggerClient.Write(companyId);

            var sessionsManager = this.GetSessionsManager();

            var tempList = new List<GetSessionResultModel>();

            foreach (var info in sessionsManager.GetSessionInfos(companyId))
            {
                tempList.Add(new GetSessionResultModel()
                {
                    SessionId = info.SessionId,
                    RelayServiceHostName = ConfigurationManager.AppSettings["HostName"],
                    ClientListenPort = info.Client.ListenPort,
                    CreationTime = info.CreationTime,
                    ExpiredTime = info.ExpiredTime,

                    SessionState = info.State,
                    ClientState = new UserState()
                    {
                        HostName = info.Client.HostName,
                        UserName = info.Client.UserName,
                    },
                    ServerState = new UserState()
                    {
                        HostName = info.Server.HostName,
                        UserName = info.Server.UserName,
                    }
                });
            }

            return tempList;
        }

        // 複数セッションの更新。
        [HttpPost]
        [ActionName("update")]
        public void Update(IEnumerable<UpdateSessionRequestModel> items)
        {
            var companyId = this.GetCompanyId();
            var sessionsManager = this.GetSessionsManager();

            foreach (var item in items)
            {
                sessionsManager.UpdateState(companyId, item.SessionId, item.State, item.HostName, item.UserName);
            }
            UpdateSessionsLoggerClient.Write(companyId, items);
        }

        // 複数セッションの削除。
        [HttpPost]
        [ActionName("delete")]
        public void Delete(IEnumerable<DeleteSessionRequestModel> items)
        {
            var companyId = this.GetCompanyId();
            var sessionsManager = this.GetSessionsManager();

            foreach (var item in items)
            {
                sessionsManager.DeleteSession(companyId, item.SessionId);
            }

            DeleteSessionsLogger.Write(companyId, items);
        }

        // セッションの削除。
        [HttpDelete]
        public void Delete([FromUri] string id)
        {
            var companyId = this.GetCompanyId();
            var sessionsManager = this.GetSessionsManager();

            sessionsManager.DeleteSession(companyId, id);
        }
    }
}
