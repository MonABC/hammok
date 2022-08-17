using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common.Loggers;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Sessions
{
    public class SessionsManager : IDisposable
    {
        private readonly HashSet<CompanyModel> _companies = new HashSet<CompanyModel>();
        private readonly Dictionary<string, Dictionary<string, SessionInfoModel>> _sessionInfoMap = new Dictionary<string, Dictionary<string, SessionInfoModel>>();
        private readonly ConcurrentDictionary<string, object> _lockObjects = new ConcurrentDictionary<string, object>();

        // 起動中のastRCrelay.exeのプロセスIDとProcessのマップ。
        private readonly Dictionary<int, Process> _processMap = new Dictionary<int, Process>();

        // 使用中のポート番号のセット。
        private readonly HashSet<int> _usingPortSet = new HashSet<int>();

        public SessionTimerLogger SessionTimerLogger { get; set; } = new SessionTimerLogger();
        public CompanyPortFinder CompanyPortFinder { get; set; } = new CompanyPortFinder();
        public LogFileCleaner LogFileCleaner { get; set; } = new LogFileCleaner();

        private readonly Random _random = new Random();

        private Timer _timer;

        private readonly object _lockObject = new object();

        private readonly string CompaniesFilePath;
        private readonly string SessionsFilePath;
        private readonly string RelayExeFilePath;

        private const int MAX_SESSIONS = 5001;
        private readonly int _maxSessions = 20;

        public SessionsManager(string basePath, int maxSessions)
        {
            var settingDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Hammock\PLATINUM\Server");
            CompaniesFilePath = Path.Combine(settingDirectoryPath, "astRC_RelayService_Companies.esf");
            SessionsFilePath = Path.Combine(settingDirectoryPath, "astRC_RelayService_Sessions.esf");
            RelayExeFilePath = Path.Combine(basePath, "astRCrelay.exe");

            // セッションIDが0000 ~ 9999で生成されるため、セッション数の最大は9999とする。
            // 使用できるポートが20000 ~ 30000の1万ポート 且つ 1セッション辺りクライアント側サーバー側で2ポート使用するため、セッション数の最大は5000とする。
            if (0 < maxSessions && maxSessions < MAX_SESSIONS)
            {
                _maxSessions = maxSessions;
            }

            _timer = new Timer((_) => this.TimerCallback());
        }

        private void TimerCallback()
        {
            lock (_lockObject)
            {
                // 古いログファイルを削除する。
                LogFileCleaner.Clean();

                // 一定以上経過したセッションを削除する。
                var now = DateTime.UtcNow;

                foreach (var info in _sessionInfoMap.SelectMany(n => n.Value.Select(m => m.Value)))
                {
                    if (info.State == SessionState.Connected)
                    {
                        // 接続中のセッションは状態が更新された時間から40分経過後に削除する。
                        if (now < info.UpdateTime + TimeSpan.FromMinutes(40)) continue;
                    }
                    else
                    {
                        // 接続されていないセッションは失効時間を過ぎた場合に削除する。
                        if (now < info.ExpiredTime) continue;
                    }

                    this.DeleteSession(info.CompanyId, info.SessionId);
                }

                SessionTimerLogger.Write(_sessionInfoMap.SelectMany(n => n.Value.Select(m => m.Value)));

                // 何らかの理由で異常終了したastRCrelay.exeを再起動する。
                foreach (var info in _sessionInfoMap.SelectMany(n => n.Value.Select(m => m.Value)).ToArray())
                {
                    Process process;

                    // _processMapに存在しないセッション情報は削除する。
                    if (!_processMap.TryGetValue(info.ProcessId, out process))
                    {
                        this.DeleteSession(info.CompanyId, info.SessionId);
                        continue;
                    }

                    // 終了している場合は、再度起動させる。
                    if (process.HasExited)
                    {
                        _processMap.Remove(info.ProcessId);
                        process.Dispose();

                        // astRCrelay.exeの起動を試みる。
                        var newProcess = this.CreateRelayProcess(info.Server.ListenPort);

                        // 起動に失敗した場合は諦める。
                        if (newProcess == null)
                        {
                            this.DeleteSession(info.CompanyId, info.SessionId);
                        }
                        else
                        {
                            // 起動に成功した場合は新しいプロセスを登録する。
                            _processMap.Add(newProcess.Id, newProcess);
                            info.ProcessId = newProcess.Id;
                        }
                    }
                }

                // 現在の状態を保存する。
                this.Save();
            }
        }

        private Process CreateRelayProcess(int serverPort)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = RelayExeFilePath;
            startInfo.Arguments = string.Format("/port:{0}", serverPort);
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            Process  process = Process.Start(startInfo);

            Thread.Sleep(1000);

            if (process.HasExited) throw new InvalidOperationException("プロセスが正常に起動していない。");

            return process;
        }

        private void FindFreePort(string companyId, out int serverPort, out int clientPort)
        {
            lock (_lockObject)
            {
                Stopwatch sw = Stopwatch.StartNew();

                for (; ; )
                {
                    // エフェメラルポートを考慮。
                    serverPort = CompanyPortFinder.Find(companyId);
                    clientPort = serverPort + 1;

                    // 重複するポートをListenしようとしている場合、リトライする。
                    if (_usingPortSet.Contains(serverPort) || _usingPortSet.Contains(clientPort))
                    {
                        continue;
                    }

                    // 実際にListenを実行しポートが使用可能か調べる。
                    try
                    {
                        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                        {
                            socket.Bind(new IPEndPoint(IPAddress.Any, serverPort));
                        }

                        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                        {
                            socket.Bind(new IPEndPoint(IPAddress.Any, clientPort));
                        }
                    }
                    catch (Exception)
                    {
                        if (sw.Elapsed.TotalSeconds > 5) throw;

                        Thread.Sleep(100);

                        continue;
                    }

                    // 一覧へポートを追加。
                    _usingPortSet.Add(serverPort);
                    _usingPortSet.Add(clientPort);

                    break;
                }
            }
        }

        public void Init()
        {
            lock (_lockObject)
            {
                this.Load();

                _timer.Change(1000 * 30, 1000 * 60);

                _processMap.Clear();
                _usingPortSet.Clear();

                // astRC_RelayServiceの再起動を考慮し、「astRCrelay.exe」の起動状況とデータベースの状態を同期させる。
                {
                    {
                        // データベースに存在するセッション情報内のプロセスIDのセット。
                        var tempSet = new HashSet<int>(_sessionInfoMap.SelectMany(n => n.Value.Select(m => m.Value)).Select(n => n.ProcessId));

                        foreach (var process in Process.GetProcessesByName("astRCrelay"))
                        {
                            // データベースに存在しない「astRCrelay.exe」プロセスを終了させる。
                            if (!tempSet.Contains(process.Id))
                            {
                                try
                                {
                                    process.Kill();
                                    process.Dispose();
                                }
                                catch (InvalidOperationException e)
                                {
                                    Logger.Error(e);
                                }

                                continue;
                            }

                            _processMap[process.Id] = process;
                        }
                    }

                    // 動作の確認できない「astRCrelay.exe」のプロセスIDを持つセッションを削除。
                    foreach (var map in _sessionInfoMap.Values)
                    {
                        foreach (var pair in map.ToArray())
                        {
                            if (!_processMap.ContainsKey(pair.Value.ProcessId))
                            {
                                map.Remove(pair.Key);
                            }
                        }
                    }
                }

                // 使用中のポートを探索。
                foreach (var sessionState in _sessionInfoMap.SelectMany(n => n.Value.Select(m => m.Value)))
                {
                    _usingPortSet.Add(sessionState.Client.ListenPort);
                    _usingPortSet.Add(sessionState.Server.ListenPort);
                }
            }
        }

        private void Load()
        {
            lock (_lockObject)
            {
                if (!File.Exists(CompaniesFilePath))
                {
                    Logger.Debug($"ファイルが存在しません={CompaniesFilePath}");
                    throw new FileNotFoundException();
                }

                _companies.Clear();
                _companies.UnionWith(SettingsFramework.Load<IEnumerable<CompanyModel>>(CompaniesFilePath));

                if (File.Exists(SessionsFilePath))
                {
                    _sessionInfoMap.Clear();

                    foreach (var info in SettingsFramework.Load<IEnumerable<SessionInfoModel>>(SessionsFilePath))
                    {
                        Dictionary<string, SessionInfoModel> map;

                        if (!_sessionInfoMap.TryGetValue(info.CompanyId, out map))
                        {
                            map = new Dictionary<string, SessionInfoModel>();
                            _sessionInfoMap[info.CompanyId] = map;
                        }

                        map[info.SessionId] = info;
                    }
                }
            }
        }

        public void Save()
        {
            lock (_lockObject)
            {
                Logger.Debug($"Session保存:{SessionsFilePath}");
                SettingsFramework.Save(SessionsFilePath, _sessionInfoMap.SelectMany(n => n.Value.Select(m => m.Value)).ToArray());
            }
        }

        public bool Authentication(string companyId, string password)
        {
            lock (_lockObject)
            {
                return _companies.Contains(new CompanyModel(companyId, password));
            }
        }

        public IEnumerable<SessionInfoModel> GetSessionInfos(string companyId)
        {
            lock (_lockObject)
            {
                Dictionary<string, SessionInfoModel> map;
                if (!_sessionInfoMap.TryGetValue(companyId, out map)) return Enumerable.Empty<SessionInfoModel>();

                return map.Values;
            }
        }

        public SessionInfoModel CreateSession(string companyId, string serverHostName, string serverUserName)
        {
            // 1つの企業が実行できるセッション作成スレッド数を1つへ制限する。
            lock (_lockObjects.GetOrAdd(companyId, (_) => new object())) // 企業ID毎にロック用のオブジェクトを生成する。
            {
                // セッション数が上限(20セッション)に達しているかどうか確認。
                lock (_lockObject)
                {
                    Dictionary<string, SessionInfoModel> map;

                    if (!_sessionInfoMap.TryGetValue(companyId, out map))
                    {
                        map = new Dictionary<string, SessionInfoModel>();
                        _sessionInfoMap[companyId] = map;
                    }

                    if (_maxSessions <= map.Count)
                    {
                        throw new TooManySessionsException();
                    }
                }

                int serverPort, clientPort; // 仕様上、astRCrelay.exeの「クライアントポート」は「サーバーポート + 1」の様である。
                this.FindFreePort(companyId, out serverPort, out clientPort); // 実際にListenして、未使用のポートを探す。

                try
                {
                    var process = this.CreateRelayProcess(serverPort);
                    if (process == null) throw new InvalidOperationException("astRCrelay.exeの起動に失敗。");

                    // マルチスレッドを考慮。
                    lock (_lockObject)
                    {
                        Dictionary<string, SessionInfoModel> map;

                        if (!_sessionInfoMap.TryGetValue(companyId, out map))
                        {
                            map = new Dictionary<string, SessionInfoModel>();
                            _sessionInfoMap[companyId] = map;
                        }

                        string sessionId = null;

                        for (; ; )
                        {
                            sessionId = String.Format("{0:0000}", _random.Next(0, MAX_SESSIONS)); // 0000 ~ 9999のセッションIDを生成。
                            if (!map.ContainsKey(sessionId)) break;
                        }

                        var now = DateTime.UtcNow;
                        var sessionInfo = new SessionInfoModel(
                            companyId,
                            sessionId,
                            System.Web.HttpContext.Current.GetClientIP(),
                            process.Id,
                            now,
                            now,
                            now.AddHours(1),
                            SessionState.Standby,
                            new UserInfoModel(clientPort, null, null),
                            new UserInfoModel(serverPort, serverHostName, serverUserName));
                        map[sessionId] = sessionInfo;

                        _processMap[process.Id] = process;

                        return sessionInfo;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);

                    lock (_lockObject)
                    {
                        _usingPortSet.Remove(serverPort);
                        _usingPortSet.Remove(clientPort);
                    }
                    throw;
                }
            }
        }

        public void UpdateState(string companyId, string sessionId, SessionState state, string hostName, string userName)
        {
            lock (_lockObject)
            {
                Dictionary<string, SessionInfoModel> map;
                if (!_sessionInfoMap.TryGetValue(companyId, out map)) return;

                SessionInfoModel sessionInfo;
                if (!map.TryGetValue(sessionId, out sessionInfo)) return;

                map[sessionId] = new SessionInfoModel(
                    sessionInfo.CompanyId,
                    sessionInfo.SessionId,
                    sessionInfo.IpAddress,
                    sessionInfo.ProcessId,
                    sessionInfo.CreationTime,
                    DateTime.UtcNow,
                    sessionInfo.ExpiredTime,
                    state,
                    new UserInfoModel(sessionInfo.Client.ListenPort, hostName, userName),
                    sessionInfo.Server);
            }
        }

        public void DeleteSession(string companyId, string sessionId)
        {
            lock (_lockObject)
            {
                Dictionary<string, SessionInfoModel> map;
                if (!_sessionInfoMap.TryGetValue(companyId, out map)) return;

                SessionInfoModel sessionInfo;
                if (!map.TryGetValue(sessionId, out sessionInfo)) return;

                Process process;

                if (_processMap.TryGetValue(sessionInfo.ProcessId, out process))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (InvalidOperationException e)
                    {
                        Logger.Error(e);
                    }

                    process.Dispose();
                    _processMap.Remove(sessionInfo.ProcessId);
                }

                _usingPortSet.Remove(sessionInfo.Client.ListenPort);
                _usingPortSet.Remove(sessionInfo.Server.ListenPort);

                map.Remove(sessionId);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
