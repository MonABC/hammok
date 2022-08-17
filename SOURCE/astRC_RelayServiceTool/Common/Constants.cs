namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    /// <summary>
    /// グローバルな定数を保持するクラス。
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// コマンドタイムアウトの初期値を定義する。
        /// </summary>
        public const int DEFAULT_COMMAND_TIMEOUT = 300;

        /// <summary>
        /// 「全て」グループのIDを定義する。
        /// </summary>
        public const string WEB_TOP_GROUP_ID = "1";

        /// <summary>
        /// ローカル保持する設定ファイル名を定義する。
        /// </summary>
        public const string LOCAL_SETTINIG_FILE_NAME = "astRC_RelayServiceTool.xml";

        /// <summary>
        /// INSERT実行の閾値
        /// </summary>
        public const int INSERT_LIMIT = 1000;

        /// <summary>
        /// AdminのManagerID
        /// </summary>
        public const int ADMIN_MANAGER_ID = 1;
    }
}