using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils
{
    internal static partial class RcUtils
    {
        // astRCC.exe多重起動防止Mutex識別子
        private const string ASTRC_CLIENT_MUTEX = "Global\\{02DDB322-013E-46A1-B156-E8365BF41426}";

        public static bool GetAssetViewClientActivationState()
        {
            // astRCC.exeが起動しているか確認する。
            try
            {
                var ret = Mutex.TryOpenExisting(ASTRC_CLIENT_MUTEX, out var rccMutex);
                rccMutex?.Dispose();
                return ret;
            }
            catch (UnauthorizedAccessException)
            {

            }

            return false;
        }

        public static void WaitForExitOfProcess(string name)
        {
            for (; ; )
            {
                bool flag = true;

                Process[] ps = Process.GetProcessesByName(name);

                if (ps.Length > 0)
                {
                    Logger.Debug(string.Format("終了待機中_{0}:{1}", name, ps.Length));
                    Thread.Sleep(1000);
                }
                else
                {
                    Logger.Debug(string.Format("{0}の終了を確認", name));
                    flag = false;
                }

                if (!flag) break;
            }
        }

        public static string GetRcPassword(DateTime expiredTime, string sessionId)
        {
            return string.Format("{0}{1}", expiredTime.ToString("MMddHHmmss"), sessionId);
        }

        public static string GetAstRcExeFilePath()
        {
            string filePath;

            // AssetViewクライアントの環境を想定。
            filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Hammock\PLATINUM\Client\RC\astRC.exe");

            if (File.Exists(filePath))
            {
                Logger.Debug(string.Format("astRC.exeファイルパス取得。(AssetViewクライアントインストール環境): \"{0}\"", filePath));
                return filePath;
            }

            // 管理コンソールインストール環境を想定。
            filePath = Path.Combine(Environment.ExpandEnvironmentVariables("%ALLUSERSPROFILE%"), @"Hammock\PLATINUM\Viewer\RC\astRC.exe");

            if (File.Exists(filePath))
            {
                Logger.Debug(string.Format("astRC.exeファイルパス取得。(管理コンソールインストール環境): \"{0}\"", filePath));
                return filePath;
            }

            // 起動中のプロセスからファイルパスを取得。
            filePath = GetProsessFilePath("astRC.exe");

            if (File.Exists(filePath))
            {
                Logger.Debug(string.Format("astRC.exeファイルパス取得。(実行中のプロセスから取得): \"{0}\"", filePath));
                return filePath;
            }

            throw new InvalidOperationException();
        }

        private static string GetProsessFilePath(string name)
        {
            foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(name)))
            {
                return process.MainModule.FileName;
            }

            throw new InvalidOperationException();
        }
    }
}
