using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common.Loggers
{
    public class UpdateSessionsLoggerClient : LoggerBase
    {
        private readonly object sync = new object();
        public override string FileNameWithoutDate => "UpdateSessions";

        public void Write(string companyId, IEnumerable<UpdateSessionRequestModel> items)
        {
            // リクエストを受けた日時
            // リクエスト元のIPアドレス
            // リクエスト元の企業ID
            // リクエストパラメーターの接続ID
            // リクエストパラメーターのホスト名
            // リクエストパラメーターのログインユーザー名
            var now = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff");
            string userIp = System.Web.HttpContext.Current.GetClientIP();
            lock (sync)
            {
                using (var writer = new StreamWriter(GetCurrentLogFilePath(), true))
                {
                    foreach (var item in items)
                    {
                        writer.WriteLine(
                            "{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                            now,
                            userIp,
                            companyId,
                            item.SessionId,
                            item.HostName,
                            item.UserName);
                    }
                }
            }
        }
    }
}
