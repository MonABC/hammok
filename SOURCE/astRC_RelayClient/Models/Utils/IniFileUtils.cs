using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils
{
    public static class IniFileUtils
    {
        private static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern uint WritePrivateProfileString(string lpAppName,
               string lpKeyName, string lpString, string lpFileName);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern uint GetPrivateProfileString(
               string lpAppName,
               string lpKeyName,
               string lpDefault,
               StringBuilder lpReturnedString,
               uint nSize,
               string lpFileName);
        }

        private const int BufferSize = 256;

        /// <summary>
        ///   指定された .ini ファイル（初期化ファイル）の指定されたセクション内にある、指定されたキーに関連付けられている文字列を取得します。
        /// </summary>
        /// <remarks>
        ///   Win32API GetPrivateProfileString 関数と同一の振る舞いを提供します。
        /// </remarks>
        /// <param name = "section">セクション名</param>
        /// <param name = "key">キー名</param>
        /// <param name = "defaultValue">既定の文字列</param>
        /// <param name = "resultValue">取得した値</param>
        /// <param name = "file">.ini ファイルの名前</param>
        /// <returns>APIの戻り値　0以外：正常終了　0：取得できない</returns>
        public static string GetValue(string section, string key, string defaultValue, string file)
        {
            StringBuilder sb = new StringBuilder(BufferSize);
            while (true)
            {
                int ret = (int)NativeMethods.GetPrivateProfileString(section, key, defaultValue, sb, (uint)sb.Capacity, file);
                if (ret == 0)
                {
                    throw new Win32Exception();
                }
                if (ret.Equals(sb.Capacity - 1) == false && ret.Equals(sb.Capacity - 2) == false)
                {
                    return sb.ToString();
                }
                sb.Capacity += BufferSize;
            }
        }

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
        public static void SetValue(string section, string key, string value, string file)
        {
            var ret = NativeMethods.WritePrivateProfileString(section, key, value, file);
            if (ret == 0)
            {
                throw new Win32Exception();
            }
        }
    }
}
