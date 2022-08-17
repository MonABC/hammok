using System;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.ExtensionMethod
{
    /// <summary>
    /// Enumに関する拡張メソッドを定義する。
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Enumをintに変換する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt<T>(this T value) where T : struct ,IConvertible
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Enumをshortに変換する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static short ToShort<T>(this T value) where T : struct ,IConvertible
        {
            return Convert.ToInt16(value);
        }

        /// <summary>
        /// intから指定した型のenumに変換する。
        /// enumに対応する値が無い場合はnullを返す。
        /// </summary>
        /// <typeparam name="T">対象のenum型を指定する。</typeparam>
        /// <param name="value">対象の値を指定する。</param>
        /// <returns>変換結果のenumを返す。</returns>
        public static T? ToEnum<T>(this int? value) where T : struct ,IConvertible
        {
            //型がEnumでない場合はnullとする。
            var type = typeof(T);
            if (!type.IsEnum) return null;

            //nullの場合はnullとする。
            if (!value.HasValue) return null;

            //入力値と一致する値を返す。
            foreach (T enumValue in Enum.GetValues(type))
            {
                if (Convert.ToInt32(enumValue) == value.Value) return enumValue;
            }

            //一致する値が無ければnullとする。
            return null;
        }

        /// <summary>
        /// intから指定した型のenumに変換する。
        /// enumに対応する値が無い場合はnullを返す。
        /// </summary>
        /// <typeparam name="T">対象のenum型を指定する。</typeparam>
        /// <param name="value">対象の値を指定する。</param>
        /// <returns>変換結果のenumを返す。</returns>
        public static T? ToEnum<T>(this int value) where T : struct ,IConvertible
        {
            return ((int?)value).ToEnum<T>();
        }


        /// <summary>
        /// intから指定した型のenumに変換する。
        /// enumに対応する値が無い場合はnullを返す。
        /// </summary>
        /// <typeparam name="T">対象のenum型を指定する。</typeparam>
        /// <param name="value">対象の値を指定する。</param>
        /// <returns>変換結果のenumを返す。</returns>
        public static T? ToEnum<T>(this short? value) where T : struct ,IConvertible
        {
            return ((int?)value).ToEnum<T>();
        }

        /// <summary>
        /// intから指定した型のenumに変換する。
        /// enumに対応する値が無い場合はnullを返す。
        /// </summary>
        /// <typeparam name="T">対象のenum型を指定する。</typeparam>
        /// <param name="value">対象の値を指定する。</param>
        /// <returns>変換結果のenumを返す。</returns>
        public static T? ToEnum<T>(this short value) where T : struct ,IConvertible
        {
            return ((short?)value).ToEnum<T>();
        }
    }
}
