using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.WebApi;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Properties;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models
{
    internal partial class RcClientManager
    {
        private readonly ClientWebApi _webApi;
        private readonly string _rcExeFilePath;

        public RcClientManager(ClientWebApi webApi)
        {
            _webApi = webApi;
            _rcExeFilePath = RcUtils.GetAstRcExeFilePath();
        }

        public void Run(string sessionId)
        {
            // 30分毎にアライブを飛ばす。
            using (var timer = new Timer((_) =>
            {
                _webApi.UpdateSessions(new[] { new UpdateSessionRequestModel() { SessionId = sessionId, State = SessionState.Connected, HostName = Environment.MachineName, UserName = Environment.UserName } });
            }))
            {
                RunInternal(timer, sessionId);
            }
        }

        private void RunInternal(Timer timer, string sessionId)
        {
            try
            {
                var getSessionResultModel = _webApi.GetSessionList().FirstOrDefault(n => n.SessionId == sessionId);
                if (getSessionResultModel == null)
                {
                    throw new RCRelayClientException(string.Format(Resources.ClientWindow_ConnectionIdDoesNotExists, sessionId));
                }

                string iniFilePath = Path.Combine(Path.GetDirectoryName(_rcExeFilePath), "astRC.ini");

                try
                {
                    // iniのClientModeセクションを上書きする。
                    Logger.Debug(string.Format("astRC.iniファイルのClientModeセクションの変更"));

                    string section = "ClientMode";
                    IniFileUtils.SetValue(section, "boot", "1", iniFilePath);
                    IniFileUtils.SetValue(section, "ip", getSessionResultModel.RelayServiceHostName, iniFilePath);
                    IniFileUtils.SetValue(section, "port", getSessionResultModel.ClientListenPort.ToString(), iniFilePath);
                    IniFileUtils.SetValue(section, "password", RcUtils.GetRcPassword(getSessionResultModel.ExpiredTime, getSessionResultModel.SessionId), iniFilePath);

                    _ = timer.Change(0, 1000 * 60 * 30);

                    var startInfo = new ProcessStartInfo()
                    {
                        FileName = _rcExeFilePath,
                        Arguments = string.Format("/shared /emulate3 /japkeyboard {0}", getSessionResultModel.RelayServiceHostName)
                    };

                    using (var process = Process.Start(startInfo))
                    {
                        Logger.Debug(string.Format("astRC.exeを起動。"));
                        process.WaitForExit();
                    }
                }
                finally
                {
                    // IniのClientModeセクションを削除。
                    Logger.Debug(string.Format("astRC.iniファイルのClientModeセクションを削除"));

                    string section = "ClientMode";
                    IniFileUtils.SetValue(section, null, null, iniFilePath);
                }
            }
            finally
            {
                _webApi.UpdateSessions(new[] { new UpdateSessionRequestModel() { SessionId = sessionId, State = SessionState.Standby } });
            }
        }
    }
}
