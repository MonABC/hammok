using System.Runtime.InteropServices;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    /// <summary>
    /// INIファイルの解釈を行い、値を取得するクラス。
    /// </summary>
    public class IniFileParser
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        /// <summary>
        /// ファイル名とセクションとキーを指定して値を取得する。
        /// </summary>
        /// <param name="sAppName"></param>
        /// <param name="sKeyName"></param>
        /// <param name="sDefault"></param>
        /// <param name="sFileName"></param>
        /// <returns></returns>
        public string GetValue(string sAppName, string sKeyName, string sDefault, string sFileName)
        {
            var iSize = 0;
            StringBuilder sb;
            uint uiLen;
            do
            {
                iSize = iSize + 256;
                sb = new StringBuilder(iSize);
                uiLen = GetPrivateProfileString(sAppName, sKeyName, sDefault, sb, (uint)sb.Capacity, sFileName);
            }
            while (uiLen == iSize - 1);
            return sb.ToString();
        }
    }
}