using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Dao
{
    /// <summary>
    /// xmlへのシリアライズ・デシリアライズを行うDao。
    /// </summary>
    public class XmlSerializeDao : IXmlSerializeDao
    {
        #region IXmlSerializeDao メンバー

        /// <summary>
        /// シリアライズされたxmlファイルをデシリアライズし、クラスを復元する。
        /// デシリアライズに失敗した場合はnullを返す。
        /// </summary>
        /// <typeparam name="T">対象の型を指定する。</typeparam>
        /// <param name="filePath">対象のxmlファイルを指定する。</param>
        /// <returns>デシリアライズされたクラスを返す。</returns>
        public T Read<T>(string filePath) where T : class
        {
            return Read(filePath, typeof (T)) as T;
        }

        /// <summary>
        /// シリアライズされたxmlファイルをデシリアライズし、クラスを復元する。
        /// デシリアライズに失敗した場合はnullを返す。
        /// </summary>
        /// <param name="filePath">対象のxmlファイルを指定する。</param>
        /// <param name="type">対象の型を指定する。</param>
        /// <returns>デシリアライズされたクラスを返す。</returns>
        public object Read(string filePath, Type type)
        {
            //対象のファイルが存在しなければ失敗とする。
            if (!File.Exists(filePath)) return null;

            //デシリアライズしたクラスを返す。
            var serializer = new XmlSerializer(type);
            using (var stream = new StreamReader(filePath, Encoding.Unicode, true))
            {
                return serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// クラスをシリアライズし、xmlファイルに保存する。
        /// ファイルが既に存在している場合は上書きとする。
        /// </summary>
        /// <typeparam name="T">対象の型を指定する。</typeparam>
        /// <param name="source">シリアライズするクラスを指定する。</param>
        /// <param name="filePath">保存先のxmlファイルを指定する。</param>
        /// <returns>シリアライズに成功したらtrueを返す。</returns>
        public bool Write<T>(T source, string filePath) where T : class
        {
            return Write(source, filePath, typeof (T));
        }

        /// <summary>
        /// クラスをシリアライズし、xmlファイルに保存する。
        /// ファイルが既に存在している場合は上書きとする。
        /// </summary>
        /// <param name="source">シリアライズするクラスを指定する。</param>
        /// <param name="fileFullPath">保存先のxmlファイルをフルパスで指定する。</param>
        /// <param name="type">対象の型を指定する。</param>
        /// <returns>シリアライズに成功したらtrueを返す。</returns>
        public bool Write(object source, string fileFullPath, Type type)
        {
            //フォルダーが無ければ作成する。
            var directoryPath = Path.GetDirectoryName(fileFullPath);
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            //シリアライズを行う。
            var serializer = new XmlSerializer(type);
            using (var stream = new StreamWriter(fileFullPath, false, Encoding.Unicode))
            {
                serializer.Serialize(stream, source);
            }

            return true;
        }

        #endregion
    }
}
