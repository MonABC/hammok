using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common.Loggers
{
    public class SessionTimerLogger : LoggerBase
    {
        private readonly double _timerLogInterval;
        private readonly object sync = new object();
        private DateTime _nextLogTime;
        public SessionTimerLogger()
        {
            _timerLogInterval = double.Parse(ConfigurationManager.AppSettings["TimerLogInterval"]);
             // 初回のログ出力は早めに行うものとする。
            _nextLogTime = DateTime.Now.AddMinutes(-1);
        }

        public override string FileNameWithoutDate => "SessionsLog_Timer";

        public void Write(IEnumerable<SessionInfoModel> sessions)
        {
            if (_nextLogTime > DateTime.Now)
            {
                return;
            }

            lock (sync)
            {
                if (_nextLogTime > DateTime.Now)
                {
                    return;
                }
                _nextLogTime = DateTime.Now.AddMinutes(_timerLogInterval);
                var now = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff");
                using (var writer = new StreamWriter(GetCurrentLogFilePath(), true))
                {
                    foreach (var item in sessions)
                    {
                        // ログ出力日時
                        // 企業ID
                        // 接続ID
                        // WebAPI「接続待ちを開始」時に取得したIPアドレス。
                        // 接続待機しているPCでリッスンしているポート番号
                        writer.WriteLine(
                            "{0}\t{1}\t{2}\t{3}\t{4}",
                            now,
                            item.CompanyId,
                            item.SessionId,
                            item.IpAddress,
                            item.Server.ListenPort);
                    }
                }
            }
        }
    }
}