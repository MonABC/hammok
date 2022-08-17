using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.WebApi;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Server;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models
{
    internal partial class RcServerManager
    {
        private readonly ServerWebApi _webApi;
        private readonly string _rcExeFilePath;
        private readonly RccManager _rccManager;

        private string _sessionId;

        public RcServerManager(ServerWebApi webApi)
        {
            _webApi = webApi;
            _rcExeFilePath = RcUtils.GetAstRcExeFilePath();
            _rccManager = new RccManager();
        }

        private static string EscapeCommandLineString(string value)
        {
            var sb = new StringBuilder();

            foreach (var c in value)
            {
                switch (c)
                {
                    case '\\':
                        _ = sb.Append("\\\\");
                        break;
                    case '"':
                        _ = sb.Append("\\\"");
                        break;
                    default:
                        _ = sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        private bool _sessionCreated;

        private CreateSessionResultModel CreateSession()
        {
            // WebApiへセッションの作成を依頼。
            var result = _webApi.CreateSession(new CreateSessionRequestModel()
            {
                HostName = Environment.MachineName,
                UserName = Environment.UserName
            });

            _sessionId = result.SessionId;
            _sessionCreated = true;
            return result;
        }

        private bool _serviceStarted;
        private void StartService(string hostName, int port, string password)
        {
            // RCC.exeにサービス停止要求を送信
            _rccManager.StopAstRC();

            // astRC.exeが完全に終了したことを確認する。
            RcUtils.WaitForExitOfProcess("astRC");

            // Relayモードのパスワードを設定する。
            {
                Logger.Debug("リレーモードのastRC.exeのパスワードを設定する。");

                var startInfo = new ProcessStartInfo();
                startInfo.FileName = _rcExeFilePath;
                startInfo.Arguments = string.Format("/relayip:{0} /relayport:{1} /password:\"{2}\"", hostName, port, EscapeCommandLineString(password));
                startInfo.WorkingDirectory = Path.GetDirectoryName(_rcExeFilePath);

                using (var process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            // astRC.exeが完全に終了したことを確認する。
            RcUtils.WaitForExitOfProcess("astRC");

            // Relay設定書き出し
            string iniFilePath = Path.Combine(Path.GetDirectoryName(_rcExeFilePath), "astRC_RelaySetting.ini");
            if (!File.Exists(iniFilePath))
            {
                using (FileStream fs = File.Create(iniFilePath))
                {
                    fs.Close();
                }
                Logger.Debug(string.Format("astRC_RelaySetting.iniファイル新規作成"));
            }

            Logger.Debug(string.Format("astRC_RelaySetting.iniファイルのRelaySettingセクションの変更"));

            string section = "RelaySetting";
            IniFileUtils.SetValue(section, "relayip", hostName, iniFilePath);
            IniFileUtils.SetValue(section, "relayport", port.ToString(), iniFilePath);

            // RCC.exeにサービス起動要求を送信
            _rccManager.StartAstRC();
            _serviceStarted = true;
        }

        private void DeleteSection()
        {
            string iniFilePath = Path.Combine(Path.GetDirectoryName(_rcExeFilePath), "astRC_RelaySetting.ini");

            // IniのRelaySettingセクションを削除。
            Logger.Debug(string.Format("astRC_RelaySetting.iniファイルのRelaySettingセクションを削除"));
            string section = "RelaySetting";
            IniFileUtils.SetValue(section, null, null, iniFilePath);
        }

        public CreateSessionResultModel Start()
        {
            var result = CreateSession();
            StartService(result.RelayServiceHostName, result.ServerListenPort, RcUtils.GetRcPassword(result.ExpiredTime, result.SessionId));
            return result;
        }

        private void DeleteSession()
        {
            if (_sessionId != null)
            {
                _webApi.DeleteSession(_sessionId);
            }
        }

        public void Stop()
        {
            // 既存のサービスを終了する。
            if (_serviceStarted)
            {
                _rccManager.StopAstRC();

                // astRC.exeが完全に終了したことを確認する。
                RcUtils.WaitForExitOfProcess("astRC");
                _serviceStarted = false;
            }

            _rccManager.RelayEnd();
            DeleteSection();

            if (_sessionCreated)
            {
                DeleteSession();
                _sessionCreated = false;
            }
        }
    }
}
