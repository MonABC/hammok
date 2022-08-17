namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    /// <summary>
    /// メッセージを保持していることを示すインターフェース。
    /// </summary>
    public interface IHaveMessage
    {
        /// <summary>
        /// メッセージを取得する。
        /// </summary>
        string Message { get; } 
    }
}