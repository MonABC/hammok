namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Enums
{
    /// <summary>
    /// アプリケーションの終了コードを定義する。
    /// </summary>
    public enum ExitCodes
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 引数不正
        /// </summary>
        InvalidArgument = 10,

        /// <summary>
        /// 必要ファイルなどが存在しない
        /// </summary>
        InvalidCondition = 11,

        /// <summary>
        /// 多重起動
        /// </summary>
        Duplicate = 20,

        /// <summary>
        /// 失敗
        /// </summary>
        Fail = 30,

        /// <summary>
        /// 例外発生
        /// </summary>
        Exception = 99,
    }
}