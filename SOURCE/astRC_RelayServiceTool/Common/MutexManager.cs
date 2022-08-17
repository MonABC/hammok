using System;
using System.Threading;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    /// <summary>
    /// mutexの管理を行うクラス。
    /// </summary>
    public class MutexManager
    {
        private static Mutex _mutex = null; 

        /// <summary>
        /// mutexを作成する。
        /// 作成に失敗した場合や既に作成されていた場合はfalseを返す。
        /// </summary>
        /// <returns></returns>
        public static bool CreateMutex()
        {
            try
            {
                _mutex = new Mutex(false, @"Global\{51F2F614-CB14-4948-B32D-E6DAFE04DCE4}");
            }
            catch (Exception)
            {
                //mutex作成でエラーになったらfalseを返す。
                _mutex = null;
                return false;
            }

            if (!_mutex.WaitOne(0, false)) return false;

            return true;
        }

        /// <summary>
        /// mutexを解放する。
        /// </summary>
        public static void ReleaseMutex()
        {
            if(_mutex==null) return;
            try
            {
                //mutexを解放する。
                _mutex.ReleaseMutex();
                _mutex.Close();
            }
            catch (Exception)
            {
                //解放時に発生した例外は握りつぶす。
            }
            finally
            {
                //mutexの参照を破棄する。
                _mutex = null;
            }

        }
    }
}