using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils;
using System;
using System.Threading;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models
{
    public sealed class StartupManager : IDisposable
    {
        private Mutex _astRC_RelayClient_Mutex;

        private bool _isDisposed = false;

        public bool Run()
        {
            try
            {
                // astRC_RelayClient.exeの多重起動を防止する。
                _astRC_RelayClient_Mutex = new Mutex(false, "Global\\{29E32D9A-E169-430A-A621-8DB97E44F0FA}");
                Logger.Debug("astRC_RelayClient.exeの多重起動防止のためのMutexを取得する。");
                return _astRC_RelayClient_Mutex.WaitOne(0, false);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error("多重起動を検出しました。同一PCで他のユーザーが既に本プログラムを起動していると思われます。", ex);
                return false;
            }
            catch (AbandonedMutexException e) // astRC_RelayClient.exeが異常終了後、再度起動されるときに例外が発生する。（Mutexは正しく取得できているため無視。）
            {
                Logger.Debug("異常終了検知", e);
                return true;
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            Logger.Debug("終了処理。");

            if (_astRC_RelayClient_Mutex != null)
            {
                _astRC_RelayClient_Mutex.Dispose();
                _astRC_RelayClient_Mutex = null;
            }
        }
    }
}
