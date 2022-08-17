namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Field
{
    public class LocalSettingField
    {
        /// <summary>
        /// デフォルトファイル出力先を保持する。
        /// </summary>
        public string DefaultOutputFilePath { get; set; } = string.Empty;

        /// <summary>
        /// デバッグモード
        /// </summary>
        public bool IsDebug { get; set; }

        /// <summary>
        /// Microsoft Webhook URLを保持する。
        /// </summary>
        public string MicrosoftWebhookUrl { get; set; }

        /// <summary>
        /// Teamsへの通知を停止するフラグを保持する。
        /// </summary>
        public bool StopTeams { get; set; }
    }
}
