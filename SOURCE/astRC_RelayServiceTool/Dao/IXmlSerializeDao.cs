using System;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Dao
{
    /// <summary>
    /// xmlへのシリアライズ・デシリアライズを行うDao。
    /// </summary>
    public interface IXmlSerializeDao
    {
        /// <summary>
        /// シリアライズされたxmlファイルをデシリアライズし、クラスを復元する。
        /// デシリアライズに失敗した場合はnullを返す。
        /// </summary>
        /// <typeparam name="T">対象の型を指定する。</typeparam>
        /// <param name="filePath">対象のxmlファイルを指定する。</param>
        /// <returns>デシリアライズされたクラスを返す。</returns>
        T Read<T>(string filePath) where T : class;

        /// <summary>
        /// シリアライズされたxmlファイルをデシリアライズし、クラスを復元する。
        /// デシリアライズに失敗した場合はnullを返す。
        /// </summary>
        /// <param name="filePath">対象のxmlファイルを指定する。</param>
        /// <param name="type">対象の型を指定する。</param>
        /// <returns>デシリアライズされたクラスを返す。</returns>
        object Read(string filePath, Type type);

        /// <summary>
        /// クラスをシリアライズし、xmlファイルに保存する。
        /// ファイルが既に存在している場合は上書きとする。
        /// </summary>
        /// <typeparam name="T">対象の型を指定する。</typeparam>
        /// <param name="source">シリアライズするクラスを指定する。</param>
        /// <param name="fileFullPath">保存先のxmlファイルをフルパスで指定する。</param>
        /// <returns>シリアライズに成功したらtrueを返す。</returns>
        bool Write<T>(T source, string fileFullPath) where T : class;

        /// <summary>
        /// クラスをシリアライズし、xmlファイルに保存する。
        /// ファイルが既に存在している場合は上書きとする。
        /// </summary>
        /// <param name="source">シリアライズするクラスを指定する。</param>
        /// <param name="fileFullPath">保存先のxmlファイルをフルパスで指定する。</param>
        /// <param name="type">対象の型を指定する。</param>
        /// <returns>シリアライズに成功したらtrueを返す。</returns>
        bool Write(object source, string fileFullPath, Type type);
    }
}