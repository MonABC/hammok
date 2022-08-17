using System;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.ExtensionMethod
{
    /// <summary>
    /// 文字列の拡張メソッドを定義する。
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 文字列を復号化する。
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        public static string Decrypt(this string sData)
        {
            return Hammock.AssetView.Platinum.Tools.RC.RelayService.Common.Encrypt.DecryptToString(sData);
        }

        /// <summary>
        /// 文字列を暗号化する。
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        public static string Encrypt(this string sData)
        {
            return Hammock.AssetView.Platinum.Tools.RC.RelayService.Common.Encrypt.EncryptToString(sData);
        }

        /// <summary>
        /// 文字列から引用符を削除する。
        /// 引用府中の二重引用符（""）は引用符（”）に変換する。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveQuote(this string value)
        {
            const string QUOTE = "\"";
            const string ESCAPED_QUOTE = "\"\"";
            if (value.Length < 2) return value;
            if (!value.StartsWith(QUOTE)) return value;
            if (!value.EndsWith(QUOTE)) return value;

            if (value.Length < 3) return String.Empty;
            var result = value.Substring(1, value.Length - 2);
            return result.Replace(ESCAPED_QUOTE, QUOTE);
        }

        /// <summary>
        /// 文字列を二重引用符で囲む。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SetDoubleQuote(this string value)
        {
            return string.Format("\"{0}\"", value.Replace("\"", "\"\""));
        }

        /// <summary>
        /// 文字列を数値に変換する。
        /// 変換に失敗した場合は指定したデフォルト値を返す。
        /// </summary>
        /// <param name="valueString"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToIntWithDefault(this string valueString, int defaultValue)
        {
            int value;
            if (!int.TryParse(valueString, out value))
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// 文字列をポート番号に変換する。
        /// 変換に失敗または「0~65535」範囲外の数値が渡された場合はデフォルト値を返す。
        /// </summary>
        /// <param name="valueString"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int? ToPortNumberWithDefault(this string valueString, int? defaultValue)
        {
            int value; int? ret;
            if (!int.TryParse(valueString, out value) || value < 0 || 65535 < value)
                ret = defaultValue;
            else
                ret = value;
            return ret;
        }

        /// <summary>
        /// 文字列をポート番号に変換する。
        /// 変換に失敗または「0~65535」範囲外の数値が渡された場合は例外を投げる　
        /// </summary>
        /// <param name="valueString"></param>
        /// <returns></returns>
        public static int ToPortNumberStrictly(this string valueString)
        {
            int value;
            if (!int.TryParse(valueString, out value) || value < 0 || 65535 < value)
                throw new InvalidCastException(string.Format("文字列->ポート番号のキャストに失敗：valueString={0}", valueString));
            return value;
        }

        /// <summary>
        /// 文字列がnullまたは空文字であるか判定する
        /// </summary>
        /// <param name="value">判定対象文字列</param>
        /// <returns>文字列がnullまたは空文字の場合true。それ以外の場合はfalse。</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}