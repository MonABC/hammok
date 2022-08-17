using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Dao
{
    /// <summary>
    /// データベース処理の抽象化クラス
    /// </summary>
    public abstract class DaoBase : IDaoBase
    {
        #region コンストラクタ

        /// <summary>
        ///   コンストラクタ
        /// </summary>
        protected DaoBase(string connectionString, int commandTimeout)
        {
            QueryString = string.Empty;
            ConnectionString = connectionString;
            CommandTimeout = commandTimeout;
        }

        #endregion

        #region プロパティ

        public MySqlTransaction Transaction { get; protected set; }
        public int CommandTimeout { get; set; }
        public MySqlCommand SQLCommand { get; protected set; }
        public MySqlConnection Connection { get; protected set; }
        public string ConnectionString { get; protected set; }

        protected MySqlDataReader DbdataReader { get; set; }
        protected string QueryString { get; set; }

        #endregion

        #region メンバ変数

        protected readonly MySqlParameterList SQLParameters = new MySqlParameterList();

        #endregion

        #region 接続とトランザクションの引き継ぎ

        /// <summary>
        /// 別のDaoから接続とトランザクションを引き継ぐ。
        /// </summary>
        /// <param name="anotherDao">別のDaoを指定する。</param>
        public void SetAnotherDao(IDaoBase anotherDao)
        {
            Close();
            Connection = anotherDao.Connection;
            if (anotherDao.Transaction != null)
            {
                Transaction = anotherDao.Transaction;
            }
            CommandTimeout = anotherDao.CommandTimeout;
            ConnectionString = anotherDao.ConnectionString;
        }

        #endregion

        #region DBに接続する

        /// <summary>
        /// データベースに接続する
        /// </summary>
        public void Connect()
        {
            if (Connection == null)
            {
                Connection = new MySqlConnection(ConnectionString);
            }
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        #endregion

        #region DBを切断する

        /// <summary>
        ///   DBを切断する
        /// </summary>
        public void Close()
        {
            if (Connection == null)
            {
                return;
            }
            if (ConnectionState.Open != Connection.State)
            {
                return;
            }
            Connection.Close();
            Connection.Dispose();
            Connection = null;
        }

        #endregion

        #region トランザクション開始を実行する

        /// <summary>
        ///   トランザクション開始を実行する
        /// </summary>
        public void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.Serializable);
        }

        #endregion

        #region トランザクション開始を実行する

        /// <summary>
        ///   トランザクション開始を実行する
        /// </summary>
        /// <param name = "isolationLevel">接続トランザクションのロック動作</param>
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (DbdataReader != null)
            {
                DbdataReader.Close();
            }

            if (Transaction != null)
            {
                Transaction.Dispose();
                Transaction = null;
            }

            Connect();
            Transaction = Connection.BeginTransaction(isolationLevel);
        }

        #endregion

        #region トランザクション　コミットを実行する

        /// <summary>
        /// トランザクション　コミットを実行する
        /// </summary>
        public void Commit()
        {
            if (Transaction == null)
            {
                return;
            }
            if (DbdataReader != null)
            {
                DbdataReader.Close();
            }
            Transaction.Commit();
            Transaction.Dispose();
            Transaction = null;
        }

        #endregion

        #region トランザクション　ロールバックを実行する

        /// <summary>
        /// トランザクション　ロールバックを実行する
        /// </summary>
        public void Rollback()
        {
            if (Transaction == null)
            {
                return;
            }
            if (DbdataReader != null)
            {
                DbdataReader.Close();
            }
            Transaction.Rollback();
            Transaction.Dispose();
            Transaction = null;
        }

        #endregion

        #region Disposeを実行する

        /// <summary>
        ///   Disposeを実行する
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Rollback();
                Close();
            }
        }

        #endregion

        #region SELECT文の参照

        /// <summary>
        /// SELECT文のクエリを実行する
        /// 実行のみで出力結果の格納は：
        /// while (DbdataReader.Read()){...}で行う
        /// </summary>
        /// <param name="query">実行するクエリ</param>
        protected void RunQuery(string query)
        {
            if (DbdataReader != null)
            {
                DbdataReader.Close();
            }
            QueryString = query;
            Connect();
            SQLCommand = new MySqlCommand(QueryString, Connection);
            SQLCommand.CommandTimeout = CommandTimeout;
            if (0 < SQLParameters.Count)
            {
                SQLCommand.Parameters.AddRange(SQLParameters.ToArray());
            }
            if (Transaction != null)
            {
                SQLCommand.Transaction = Transaction;
            }
            try
            {
                DbdataReader = SQLCommand.ExecuteReader();
            }
            catch (Exception)
            {
                LogQuery(SQLCommand.CommandText, SQLCommand.Parameters.Cast<MySqlParameter>().ToList(), 1);
                throw;
            }
            LogQuery(SQLCommand.CommandText, SQLCommand.Parameters.Cast<MySqlParameter>().ToList(), 99);
        }

        #endregion

        #region 値の変換

        /// <summary>
        /// SELECT文等のデータをint型に変換する
        /// </summary>
        /// <param name="column">出力の項目名</param>
        /// <returns>int型に変換したデータ</returns>
        public int GetInt32(String column)
        {
            var data = (DbdataReader.IsDBNull(DbdataReader.GetOrdinal(column))) ? 0 : Convert.ToInt32(DbdataReader[column]);
            return data;
        }

        /// <summary>
        /// SELECT文等のデータをshort型に変換する
        /// </summary>
        /// <param name="column">出力の項目名</param>
        /// <returns>short型に変換したデータ</returns>
        public short GetInt16(String column)
        {
            var data = (DbdataReader.IsDBNull(DbdataReader.GetOrdinal(column))) ? (short)0 : (short)DbdataReader[column];
            return data;
        }

        /// <summary>
        /// SELECT文等のデータをfloat型に変換する
        /// </summary>
        /// <param name="column">出力の項目名</param>
        /// <returns>float型に変換したデータ</returns>
        public float GetFloat(String column)
        {
            var data = (DbdataReader.IsDBNull(DbdataReader.GetOrdinal(column))) ? 0 : float.Parse(DbdataReader[column].ToString());
            return data;
        }

        /// <summary>
        /// SELECT文等のデータをbool型に変換する
        /// </summary>
        /// <param name="column">出力の項目名</param>
        /// <returns>bool型に変換したデータ</returns>
        public bool GetBoolean(String column)
        {
            var data = !(DbdataReader.IsDBNull(DbdataReader.GetOrdinal(column))) && (bool)DbdataReader[column];
            return data;
        }

        /// <summary>
        /// SELECT文等のデータをstring型に変換する
        /// </summary>
        /// <param name="column">出力の項目名</param>
        /// <returns>string型に変換したデータ</returns>
        public string GetString(String column)
        {
            var data = (DbdataReader.IsDBNull(DbdataReader.GetOrdinal(column))) ? "" : DbdataReader[column].ToString();
            return data;
        }

        /// <summary>
        /// SELECT文等のデータをDateTime型に変換する
        /// </summary>
        /// <param name="column">出力の項目名</param>
        /// <returns>DateTime型に変換したデータ</returns>
        public DateTime GetDateTime(String column)
        {
            var data = (DbdataReader.IsDBNull(DbdataReader.GetOrdinal(column))) ? DateTime.MinValue : (DateTime)DbdataReader[column];
            return data.AddHours(9);
        }

        /// <summary>
        /// SELECT文等のデータをnull許容型intに変換する
        /// </summary>
        /// <param name="column">出力の項目名</param>
        /// <returns>null許容型intに変換したデータ</returns>
        public int? GetNullableInt32(String column)
        {
            var data = (DbdataReader.IsDBNull(DbdataReader.GetOrdinal(column))) ? null : (int?)Convert.ToInt32(DbdataReader[column]);
            return data;
        }

        /// <summary>
        /// SELECT文等のデータをnull許容型shortに変換する
        /// </summary>
        /// <param name="column">出力の項目名</param>
        /// <returns>null許容型shortに変換したデータ</returns>
        public short? GetNullableInt16(String column)
        {
            var data = (DbdataReader.IsDBNull(DbdataReader.GetOrdinal(column))) ? null : (short?)DbdataReader[column];
            return data;
        }

        /// <summary>
        /// SELECT文等のデータをnull許容型shortに変換する
        /// </summary>
        /// <param name="column">出力の項目名</param>
        /// <returns>null許容型shortに変換したデータ</returns>
        public float? GetNullableFloat(String column)
        {
            var data = (DbdataReader.IsDBNull(DbdataReader.GetOrdinal(column))) ? null : (float?)DbdataReader[column];
            return data;
        }

        /// <summary>
        /// SELECT文等のデータをnull許容型boolに変換する
        /// </summary>
        /// <param name="column">出力の項目名</param>
        /// <returns>null許容型intに変換したデータ</returns>
        public bool? GetNullableBoolean(String column)
        {
            var data = (DbdataReader.IsDBNull(DbdataReader.GetOrdinal(column))) ? null : (bool?)DbdataReader[column];
            return data;
        }

        /// <summary>
        /// SELECT文等のデータをnull許容型DateTimeに変換する
        /// </summary>
        /// <param name="column">出力の項目名</param>
        /// <returns>null許容型intに変換したデータ</returns>
        public DateTime? GetNullableDateTime(String column)
        {
            var data = (DbdataReader.IsDBNull(DbdataReader.GetOrdinal(column))) ? null : (DateTime?)DbdataReader[column];
            return data;
        }

        #endregion

        #region パラメーター処理用のユーティリティ関数

        /// <summary>
        /// パラメータ名からパラメータの値を取得する。
        /// 取得できないばあいはnullを返す。
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        protected object GetParameterValue(string parameterName)
        {
            var targetParameter = SQLParameters.FirstOrDefault(p => p.ParameterName == parameterName);
            if (targetParameter == null) return null;
            return targetParameter.Value;
        }

        /// <summary>
        /// NVarChar型のパラメータについて全てTrimをかける。
        /// </summary>
        protected void TrimNVarCharParameters()
        {
            foreach (var parameter in SQLParameters)
            {
                //NVarChar以外の型は処理しない。
                if (parameter.MySqlDbType != MySqlDbType.VarChar) continue;

                //入力に使用しないパラメータは処理しない。
                if (parameter.Direction != ParameterDirection.Input && parameter.Direction != ParameterDirection.InputOutput) continue;

                //値がnullの場合は処理しない。
                if (parameter.Value == null) continue;

                //文字列をトリムする。
                parameter.Value = parameter.Value.ToString().Trim();
            }
        }

        #endregion

        #region ログ出力

        /// <summary>
        /// クエリをログに出力する。
        /// </summary>
        /// <param name="query">出力するクエリ文字列を指定する。</param>
        /// <param name="parameters">クエリに付随する他パラメータ一覧を指定する。</param>
        /// <param name="logLevel">出力するログレベルを指定する。</param>
        private static void LogQuery(string query, IList<MySqlParameter> parameters, int logLevel)
        {
            //出力が不要な場合は文字列の生成も行わない。
            if (Variables.LogLevel < logLevel) return;

            var log = new StringBuilder();
            //ログレベルが1の場合は表題を「例外の発生したクエリ」とする。
            log.AppendLine(logLevel == 1 ? "例外の発生したクエリ" : "実行したクエリ");
            log.AppendLine(query);

            //パラメータが1件でもある場合はパラメータの一覧を出力する。
            if (parameters.Any())
            {
                log.AppendLine("ParameterName\tDirection\tMySqlDbType\tValue");
                foreach (var parameter in parameters)
                {
                    log.Append(parameter.ParameterName);
                    log.Append("\t");
                    log.Append(parameter.Direction);
                    log.Append("\t");
                    log.Append(parameter.MySqlDbType);
                    log.Append("\t");
                    log.AppendLine(parameter.Value == null ? string.Empty : parameter.Value.ToString());
                }
            }

            ProgramLogWriter.Write(logLevel, log.ToString());
        }

        #endregion
    }
}