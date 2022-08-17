using System.Runtime.InteropServices;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    public class IniFileWriter
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern uint WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        /// <summary>
        ///   指定された .ini ファイル（初期化ファイル）の、指定されたセクション内に、指定されたキー名とそれに関連付けられた文字列を格納します。
        /// </summary>
        /// <remarks>
        ///   Win32API WritePrivateProfileString 関数と同一の振る舞いを提供します。
        /// </remarks>
        /// <param name = "section">セクション名</param>
        /// <param name = "key">キー名</param>
        /// <param name = "value">格納する文字列</param>
        /// <param name = "file">.ini ファイルの名前</param>
        /// <returns>Win32API WritePrivateProfileString 関数の戻り値</returns>
        public int SetValue(string section, string key, string value, string file)
        {
            return (int)WritePrivateProfileString(section, key, value, file);
        }
    }
}