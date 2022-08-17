using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common.Loggers
{
    public class CreateNewSessionLoggerServer : LoggerBase
    {
        private readonly object sync = new object();
        public override string FileNameWithoutDate => "CreateSession";

        public void Write(string companyId, CreateSessionRequestModel parameter, CreateSessionResultModel createdSession)
        {
            var now = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff");
            string userIp = System.Web.HttpContext.Current.GetClientIP();
            lock (sync)
            {
                using (var writer = new StreamWriter(GetCurrentLogFilePath(), true))
                {
                    // リクエストを受けた日時
                    // リクエスト元のIPアドレス
                    // リクエスト元の企業ID
                    // リクエストパラメーターのホスト名
                    // リクエストパラメーターのWindowsログオンユーザー名
                    // レスポンスの接続ID
                    // レスポンスのホスト名
                    // レスポンスのポート番号
                    // レスポンスの破棄される日時

                    writer.WriteLine(
                        "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}",
                        now,
                        userIp,
                        companyId,
                        parameter.HostName,
                        parameter.UserName,
                        createdSession.SessionId,
                        createdSession.RelayServiceHostName,
                        createdSession.ServerListenPort,
                        createdSession.ExpiredTime);
                }
            }
        }
    }
}

