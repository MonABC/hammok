namespace Hammock.AssetView.Platinum.RC.Azure.Utils
{
    public class AzureFunctionSettings
    {
        // 古いセッション情報の削除
        public static string SESSION_CONNECTED_FLAG_WAITING = "0";
        public static int SESSION_CONNECTED_FLAG_WAITING_HOUR = 48;

        public static string SESSION_CONNECTED_FLAG_CONNECTING = "1";
        public static int SESSION_CONNECTED_FLAG_CONNECTING_HOUR = 1;

        // 論理削除された企業IDの物理削除する日
        public static int COMPANY_DELETED_DATETIME_DAY = 65;

        // 実行日のXXX日先まで「T_PRC_RELAY_SERVICE_LOG_YYYYMMDD」を作成する
        public static int LOG_TABLE_CREATE_DAY = 7;

        public static int LOG_TABLE_DETELE_DAY = 375;
    }
}
