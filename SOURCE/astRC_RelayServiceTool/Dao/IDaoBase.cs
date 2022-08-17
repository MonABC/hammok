using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Dao
{
    /// <summary>
    /// Daoの基底クラスのインターフェース。
    /// </summary>
    public interface IDaoBase : IDisposable
    {
        /// <summary>
        /// コマンドのタイムアウト時間を保持する。
        /// </summary>
        int CommandTimeout { get; set; }
        
        /// <summary>
        /// 現在のトランザクションを取得する。
        /// </summary>
        MySqlTransaction Transaction { get; }

        /// <summary>
        /// 接続情報を取得する。
        /// </summary>
        MySqlConnection Connection { get;}

        /// <summary>
        /// 接続文字列を取得する。
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// データベースに接続する
        /// </summary>
        void Connect();

        /// <summary>
        ///   DBを切断する
        /// </summary>
        void Close();

        /// <summary>
        ///   トランザクション開始を実行する
        /// </summary>
        void BeginTransaction();

        /// <summary>
        ///   トランザクション開始を実行する
        /// </summary>
        /// <param name = "isolationLevel">接続トランザクションのロック動作</param>
        void BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// トランザクション　コミットを実行する
        /// </summary>
        void Commit();

        /// <summary>
        /// トランザクション　ロールバックを実行する
        /// </summary>
        void Rollback();

        /// <summary>
        /// 別のDaoから接続とトランザクションを引き継ぐ。
        /// </summary>
        /// <param name="anotherDao">別のDaoを指定する。</param>
        void SetAnotherDao(IDaoBase anotherDao);
    }
}