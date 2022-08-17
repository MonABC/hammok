using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    /// <summary>
    /// グローバル変数を保持するクラス。
    /// </summary>
    public static class Variables
    {
        /// <summary>
        /// DB（SQL Server）への接続情報を保持する。
        /// </summary>
        public static SqlConnectionStringBuilder CloudConnectionStringBuilder { get; set; }

        /// <summary>
        /// DB（MySQL）への接続情報を保持する。
        /// </summary>
        public static MySqlConnectionStringBuilder PlatinumConnectionStringBuilder { get; set; }

        /// <summary>
        /// SQLコマンドのタイムアウト時間を保持する。
        /// </summary>
        public static int CommandTimeOut { get; set; }

        /// <summary>
        /// ログの詳細度を保持する。
        /// </summary>
        public static int LogLevel { get; set; }

        /// <summary>
        /// デバッグモード
        /// </summary>
        public static bool IsDebug { get; set; }

        /// <summary>
        /// グローバル変数の初期化を行う。
        /// </summary>
        public static void InitializeVariables()
        {
            CloudConnectionStringBuilder = null;
            PlatinumConnectionStringBuilder = null;
            CommandTimeOut = Constants.DEFAULT_COMMAND_TIMEOUT;
            LogLevel = 1;
        }
    }
}