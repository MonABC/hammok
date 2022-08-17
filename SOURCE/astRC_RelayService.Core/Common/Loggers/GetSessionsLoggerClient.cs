using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common.Loggers
{
    /// <summary>
    /// セッション一覧を取得するAPI用のロガークラス
    /// 1日１ファイル作成して所定の場所に出力する。
    /// </summary>
    public class GetSessionsLoggerClient : LoggerBase
    {
        private readonly object sync = new object();
        public override string FileNameWithoutDate => "GetSessions";
        public void Write(string companyId)
        {
            var now = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff");
            string userIp = System.Web.HttpContext.Current.GetClientIP();

            lock (sync)
            {
                using (var writer = new StreamWriter(GetCurrentLogFilePath(), true))
                {
                    writer.WriteLine("{0}\t{1}\t{2}", now, userIp, companyId);
                }
            }
        }
    }
}