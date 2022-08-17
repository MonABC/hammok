using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils;
using System;
using System.Diagnostics;
using System.Threading;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models
{
    public class RccManager
    {
        // astRCC.exe多重起動防止Mutex識別子
        private const string ASTRC_CLIENT_MUTEX = "Global\\{02DDB322-013E-46A1-B156-E8365BF41426}";

        // astRCC.exeへastRC_RelayClient.exeが処理中であることを知らせるMutex識別子
        private const string ASTRC_RELAYCLIENT_MUTEX = "Global\\{62702B89-9674-4992-8658-A9D2B1C72416}";

        // astRCC.exeへループ処理を待機するよう要求するイベント識別子
        private const string ASTRC_CLIENT_Wait_Event = "Global\\{BC3A75FF-3E0C-44BF-B3C7-4F493AC47E8F}";

        // astRCC.exeへastRC_RelayClient.exeが起動を開始したことを知らせるイベント識別子
        private const string ASTRC_RELAYCLIENT_Start_Event = "Global\\{1BB2DABF-ABF2-4D8C-B088-912BAD409F23}";

        // astRC_RelayClientサービス開始イベント識別子
        private const string ASTRC_RELAYCLIENT_Service_Start_Event = "Global\\{187FE0C4-9C97-4E99-B273-07D59EA3AA10}";

        // astRC_RelayClientサービス停止イベント識別子
        private const string ASTRC_RELAYCLIENT_Service_Stop_Event = "Global\\{16614269-0364-4CD7-AB09-2C8C6C75468C}";

        // astRC_RelayClientサービスイベント識別子
        private const string ASTRC_RELAYCLIENT_Service_Wait_Event = "Global\\{A6C275C8-C52C-4DAB-A153-7B0E37978B80}";

        // astRC_RelayClientの操作が終了したことを通知するイベント
        private const string ASTRC_RELAYCLIENT_end_Event = "Global\\{234D47E4-7EFD-44C3-91CB-E628F497B270}";

        /// <summary>
        /// astRCC.exeとの通信を開始する。
        /// 本クラスの諸々のメソッド使用するには、先にこのメソッドを呼び出して置くこと。
        /// </summary>
        /// <exception cref="RCRelayClientException"></exception>
        /// <exception cref="TimeoutException"></exception>
        public void InitiateCommunicationWithAstRCC()
        {
            try
            {
                Logger.StartMethod();
                // astRCC.exeへプロセスが起動していることを知らせる。
                using (var _astRC_RelayClient_Mutex = new Mutex(false, ASTRC_RELAYCLIENT_MUTEX))
                {
                    try
                    {
                        Logger.Debug("astRCC.exeへastRC_RelayClient.exeが処理中であることを知らせるMutexを取得する");
                        if (!_astRC_RelayClient_Mutex.WaitOne(1000, false))
                        {
                            throw new RCRelayClientException("astRCC.exeへastRC_RelayClient.exeが処理中であることを知らせるMutexのロックを取得することが出来ませんでした。");
                        }
                    }
                    catch (AbandonedMutexException e) // astRC_RelayClient.exeが異常終了後、再度起動されるときに例外が発生する。（Mutexは正しく取得できているため無視。）
                    {
                        Logger.Debug("異常終了検知", e);
                    }

                    // astRCC.exeが起動しているか確認する。
                    if (Mutex.TryOpenExisting(ASTRC_CLIENT_MUTEX, out var rccMutex))
                    {
                        rccMutex.Dispose();
                    }
                    else
                    {
                        // Mutexが取得出来ない場合、起動していないと判断。
                        Logger.Debug("astRCC.exeの起動が確認できないため、astRCC.exeが存在しない環境と判断。");
                        Logger.Debug(string.Format("astRCC.exeの起動が確認できないため、astRCC.exeが存在しない環境と判断。astRC.exe:{0}, / astRCC.exe:{1}", Process.GetProcessesByName("astRC").Length, Process.GetProcessesByName("astRCC").Length));
                        return; // 起動していない場合は何もしない。
                    }

                    using (var astRC_RelayClient_Start_Event = new EventWaitHandle(false, EventResetMode.AutoReset, ASTRC_RELAYCLIENT_Start_Event))
                    {
                        using (var astRC_Client_Wait_Event = new EventWaitHandle(false, EventResetMode.AutoReset, ASTRC_CLIENT_Wait_Event))
                        {
                            Logger.Debug(string.Format("astRCC.exeへastRC_RelayClient.exeが処理を開始したことを知らせる。astRC.exe:{0}, / astRCC.exe:{1}", Process.GetProcessesByName("astRC").Length, Process.GetProcessesByName("astRCC").Length));
                            _ = astRC_RelayClient_Start_Event.Set();

                            Logger.Debug(string.Format("astRCC.exeが待機状態へ移行するまで待機。astRC.exe:{0}, / astRCC.exe:{1}", Process.GetProcessesByName("astRC").Length, Process.GetProcessesByName("astRCC").Length));

                            if (!astRC_Client_Wait_Event.WaitOne(120000))
                            {
                                throw new TimeoutException(string.Format("astRCC.exeを待機モードへ移行させることができませんでした。astRC.exe:{0}, / astRCC.exe:{1}", Process.GetProcessesByName("astRC").Length, Process.GetProcessesByName("astRCC").Length));
                            }

                            Logger.Debug("astRCC.exeが待機状態へ移行したことを確認。");
                            return;
                        }
                    }
                }
            }
            finally
            {
                Logger.EndMethod();
            }
        }

        /// <summary>
        /// astRC.exeを開始するよう、astRCC.exeに依頼する。
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        public void StartAstRC()
        {
            try
            {
                Logger.StartMethod();
                using (var astRC_RelayClient_Service_Start_Event = new EventWaitHandle(false, EventResetMode.AutoReset, ASTRC_RELAYCLIENT_Service_Start_Event))
                {
                    using (var astRC_RelayClient_Service_Wait_Event = new EventWaitHandle(false, EventResetMode.AutoReset, ASTRC_RELAYCLIENT_Service_Wait_Event))
                    {
                        Logger.Debug(string.Format("astRCC.exeへastRC_RelayClient.exeからのサービス起動依頼を送信。astRC.exe:{0}, / astRCC.exe:{1}", Process.GetProcessesByName("astRC").Length, Process.GetProcessesByName("astRCC").Length));
                        _ = astRC_RelayClient_Service_Start_Event.Set();
                        if (!astRC_RelayClient_Service_Wait_Event.WaitOne(120000))
                        {
                            // 下の例外メッセージは誰もキャッチしないため、翻訳不要。
                            // キャッチされない例外はログ出力されるのみ。
                            throw new TimeoutException(string.Format("サービスを開始させることができませんでした。astRC.exe:{0}, / astRCC.exe:{1}", Process.GetProcessesByName("astRC").Length, Process.GetProcessesByName("astRCC").Length));
                        }

                        Logger.Debug(string.Format("サービス起動要求の成功を確認。astRC.exe:{0}, / astRCC.exe:{1}", Process.GetProcessesByName("astRC").Length, Process.GetProcessesByName("astRCC").Length));
                    }
                }
            }
            finally
            {
                Logger.EndMethod();
            }
        }

        /// <summary>
        /// astRC.exeを停止するよう、astRCC.exeに依頼する。
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        public void StopAstRC()
        {
            try
            {
                Logger.StartMethod();
                using (var astRC_RelayClient_Service_Stop_Event = new EventWaitHandle(false, EventResetMode.AutoReset, ASTRC_RELAYCLIENT_Service_Stop_Event))
                {
                    using (var astRC_RelayClient_Service_Wait_Event = new EventWaitHandle(false, EventResetMode.AutoReset, ASTRC_RELAYCLIENT_Service_Wait_Event))
                    {
                        StopServiceInternal(astRC_RelayClient_Service_Stop_Event);
                        _ = astRC_RelayClient_Service_Stop_Event.Set();
                        if (!astRC_RelayClient_Service_Wait_Event.WaitOne(120000))
                        {
                            throw new TimeoutException(string.Format("サービスを再開できませんでした。 astRC.exe:{0}, / astRCC.exe:{1}", Process.GetProcessesByName("astRC").Length, Process.GetProcessesByName("astRCC").Length));
                        }

                        Logger.Debug(string.Format("サービス停止要求の成功を確認。astRC.exe:{0}, / astRCC.exe:{1}", Process.GetProcessesByName("astRC").Length, Process.GetProcessesByName("astRCC").Length), null);
                    }
                }
            }
            finally
            {
                Logger.EndMethod();
            }
        }

        private void StopServiceInternal(EventWaitHandle _astRC_RelayClient_Service_Stop_Event)
        {
            if (Process.GetProcessesByName("astRC").Length == 0)
            {
                Logger.Debug(string.Format("astRC.exeが起動していません。astRC.exe:{0}, / astRCC.exe:{1}", Process.GetProcessesByName("astRC").Length, Process.GetProcessesByName("astRCC").Length), null);
                StartAstRC();
            }

            Logger.Debug(string.Format("astRCC.exeへastRC_RelayClient.exeからのサービス停止依頼を送信。astRC.exe:{0}, / astRCC.exe:{1}", Process.GetProcessesByName("astRC").Length, Process.GetProcessesByName("astRCC").Length), null);
            _ = _astRC_RelayClient_Service_Stop_Event.Set();
            Logger.Debug(string.Format("astRCC.exe停止。astRC.exe:{0}, / astRCC.exe:{1}", Process.GetProcessesByName("astRC").Length, Process.GetProcessesByName("astRCC").Length), null);
            return;
        }

        /// <summary>
        /// astRC_RelayClient.exeの操作が完了したことを送信する
        /// </summary>
        public void RelayEnd()
        {
            try
            {
                Logger.StartMethod();
                using (var astRC_RelayClient_End_Event = new EventWaitHandle(false, EventResetMode.AutoReset, ASTRC_RELAYCLIENT_end_Event))
                {
                    Logger.Debug("astRC_RelayClient.exe 操作完了イベントを送信");
                    _ = astRC_RelayClient_End_Event.Set();
                }
            }
            finally
            {
                Logger.EndMethod();
            }
        }
    }
}
