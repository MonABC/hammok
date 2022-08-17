namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    /// <summary>
    /// 結果コードを表示用の文言に変換する。
    /// </summary>
    public interface IResultCodeTranslator
    {
        /// <summary>
        /// 結果コードを表示用の文言に変換する。
        /// </summary>
        /// <param name="resultCode">対象の結果コードを指定する。</param>
        /// <returns></returns>
        string Translate(int resultCode);
    }
}