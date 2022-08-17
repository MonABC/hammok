using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Field
{
    /// <summary>
    ///   データベース接続用の情報を格納するクラス
    /// </summary>
    public class DBConnectionSetting
    {
        /// <summary>
        ///   設定ファイル格納順
        /// </summary>
        public int SettingIndex { get; set; }

        /// <summary>
        ///   データベースサーバー名
        /// </summary>
        public string DBServerName { get; set; }

        /// <summary>
        /// データベース接続ポート番号
        /// </summary>
        public int? DBServerPort { get; set; }

        /// <summary>
        ///   データベース名
        /// </summary>
        public string DBName { get; set; }

        /// <summary>
        ///   データベース接続ユーザー名
        /// </summary>
        public string DBUserName { get; set; }

        /// <summary>
        ///   データベース接続パスワード
        /// </summary>
        public string DBPassword { get; set; }

        /// <summary>
        ///   データベース接続のタイムアウト値（秒）
        /// </summary>
        public int DBConnectionTimeOut { get; set; }

        /// <summary>
        ///   データベースへのクエリタイムアウト値（秒）
        /// </summary>
        public int DBCommandTimeOut { get; set; }

        /// <summary>
        ///   データベースのMySqlSslMode
        /// </summary>
        public int DBSslMode { get; set; }

        /// <summary>
        ///   データベースのMySqlSslMode
        /// </summary>
        public MySqlSslMode DBMySqlSslMode
        {
            get
            {
                switch (DBSslMode)
                {
                    case (int)MySqlSslMode.Preferred:
                        return MySqlSslMode.Preferred;
                    case (int)MySqlSslMode.Required:
                        return MySqlSslMode.Required;
                    case (int)MySqlSslMode.VerifyCA:
                        return MySqlSslMode.VerifyCA;
                    case (int)MySqlSslMode.VerifyFull:
                        return MySqlSslMode.VerifyFull;
                }
                return MySqlSslMode.Disabled;
            }
        }

        /// <summary>
        /// 接続文字列（SQL Server用）
        /// </summary>
        public string CloudConnectionString
        {
            get
            {
                return CloudConnectionStringBuilder.ConnectionString;
            }
        }

        /// <summary>
        /// 接続文字列（SQL Server用）
        /// </summary>
        public SqlConnectionStringBuilder CloudConnectionStringBuilder
        {
            get
            {
                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = DBServerName,
                    InitialCatalog = DBName,
                    UserID = DBUserName,
                    Password = DBPassword,
                    ConnectTimeout = DBConnectionTimeOut,
                    PersistSecurityInfo = false,
                };
                return builder;
            }
        }
    }
}
