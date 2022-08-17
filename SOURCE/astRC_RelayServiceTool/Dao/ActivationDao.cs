using Hammock.AssetView.Platinum.Tools.RC.RelayService.Field;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Dao
{
    /// <summary>
    /// アクティベーション情報等にアクセスするためのクラス。
    /// </summary>
    public class ActivationDao : PlatinumDaoBase, IActivationDao
    {
        /// <summary>
        /// DBに接続できるか確認する。
        /// </summary>
        /// <returns></returns>
        public bool CanConnect()
        {
            //接続文字列が空文字なら失敗とする。
            if (string.IsNullOrEmpty(ConnectionString)) return false;

            try
            {
                Connect();
            }
            catch (MySqlException)
            {
                //接続時に例外が発生したら失敗とする。
                return false;
            }
            finally
            {
                //接続を閉じる。
                Close();
            }

            return true;
        }

        /// <summary>
        /// アカウントのリストを取得する。
        /// </summary>
        /// <param name="serchInfo">検索条件</param>
        /// <returns></returns>
        public IList<CompanyListField> GetCompanyList(CompanyListField serchInfo = null)
        {
            var data = new List<CompanyListField>();

            // ユーザーマスタのリスト取得クエリを定義する。
            const string GET_QUERY_BASE = @"
SELECT
 COMPANY_CODE
,COMPANY_PASSWORD
FROM T_RC_RELAY_SERVICE_COMPANY_MASTER
WHERE LATEST_FLAG = 1
  AND DELETE_FLAG = 0
";
            var where = string.Empty;
            SQLParameters.Clear();
            RunQuery($"{GET_QUERY_BASE};");
            try
            {
                while (DbdataReader.Read())
                {
                    var item = new CompanyListField
                    {
                        Id = GetString("COMPANY_CODE"),
                        Password = GetString("COMPANY_PASSWORD"),
                    };

                    data.Add(item);
                }
                return data;
            }
            finally
            {
                DbdataReader.Close();
            }
        }
    }
}