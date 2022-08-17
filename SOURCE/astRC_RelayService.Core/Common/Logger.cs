using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    public static class Logger
    {
        private enum LogLevel : int
        {
            Level1 = 1,
            Level2 = 2,
            Level99 = 99
        }

        private static LogLevel _logLevel = LogLevel.Level99;
        private static string _directoryPath;
        private static string _fileNameBase;

        private const int _maxFileCount = 10;
        private const int _maxFileSize = 1 * 1024 * 1024;

        private static readonly object _lockObject = new object();

        #region メソッド(public)

        static Logger()
        {
            Initialize(@"HKEY_LOCAL_MACHINE\SOFTWARE\Hammock\PLATINUM\Server\RC", "LogLevel", Environment.SpecialFolder.CommonApplicationData, @"Hammock\PLATINUM\Server\Logs", "astRC_RelayService");
        }

        /// <summary>
        /// ログクラスの初期化を行う
        /// </summary>
        /// <param name="regKey">ログレベルの格納されているレジストリキー</param>
        /// <param name="regValue">ログレベルのレジストリエントリー名</param>
        /// <param name="specialFolder">ログ出力を行う基本ディレクトリ</param>
        /// <param name="directoryPath">ログ出力ディレクトリ（基本ディレクトリからの相対パス）</param>
        /// <param name="fileNameBase">ファイル名の元。このファイル名に『_番号.log』が付与されファイル名になる。</param>
        private static void Initialize(string regKey, string regValue, Environment.SpecialFolder specialFolder, string directoryPath, string fileNameBase)
        {
            var obj = Registry.GetValue(regKey, regValue, "99");
            if (obj != null)
            {
                _logLevel = (LogLevel)Convert.ToInt32(obj);
            }
            else
            {
                _logLevel = LogLevel.Level99;
            }

            _directoryPath = Path.Combine(Environment.GetFolderPath(specialFolder), directoryPath);
            _fileNameBase = fileNameBase;
        }

        /// <summary>
        /// ファイルの番号を振りなおし、新規に01.logファイルを作成する。10.logは削除する。
        /// </summary>
        /// <param name="logFilePathBase">ファイル名の元</param>
        private static void RotateFile(string logFilePathBase)
        {
            try
            {
                long fileSize;

                using (var fs = new FileStream(logFilePathBase + "_01.log", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    fileSize = fs.Length;
                }

                if (fileSize >= _maxFileSize)
                {
                    for (int i = _maxFileCount; i >= 1; i--)
                    {
                        string currentLogFilePath = String.Format("{0}_{1:00}.log", logFilePathBase, i);
                        string newLogFilePath = String.Format("{0}_{1:00}.log", logFilePathBase, i + 1);

                        if (File.Exists(currentLogFilePath))
                        {
                            File.Move(currentLogFilePath, newLogFilePath);

                            if (i == _maxFileCount)
                            {
                                File.Delete(newLogFilePath);
                            }
                        }
                    }
                }
            }
            catch (IOException)
            {
                // ログ出力できないだけなので、握りつぶす。
                return;
            }
        }

        /// <summary>
        /// ログ出力する。
        /// </summary>
        /// <param name="logLevel">ログレベル</param>
        /// <param name="message">出力文字列</param>
        private static void Write(LogLevel logLevel, string message, Exception exception)
        {
            try
            {

                if (_logLevel >= logLevel)
                {
                    lock (_lockObject)
                    {
                        if (!Directory.Exists(_directoryPath))
                        {
                            Directory.CreateDirectory(_directoryPath);
                        }

                        string logFilePathBase = Path.Combine(_directoryPath, _fileNameBase);
                        RotateFile(logFilePathBase);

                        using (var writer = new StreamWriter(logFilePathBase + "_01.log", true, new UTF8Encoding(false)))
                        {
                            if (exception == null)
                            {
                                writer.WriteLine(String.Format("{0} [{1}] {2}",
                                    DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"),
                                    Thread.CurrentThread.ManagedThreadId,
                                    message));
                            }
                            else
                            {
                                var sb = new StringBuilder();

                                sb.AppendLine("------ Exception Start ------");
                                sb.AppendLine(string.Format("Time: {0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")));
                                sb.AppendLine(string.Format("Thread Id: {0}", Thread.CurrentThread.ManagedThreadId));
                                sb.AppendLine(string.Format("Level: {0}", logLevel.ToString()));
                                if (!string.IsNullOrEmpty(message)) sb.AppendLine(string.Format("Message: {0}", message));

                                sb.AppendLine(string.Format("Exception Type: {0}", exception.GetType().FullName.ToString()));
                                sb.AppendLine(string.Format("Exception Message: {0}", exception.Message));
                                sb.AppendLine(string.Format("Exception StackTrace: {0}", exception.StackTrace.ToString()));

                                if (exception.InnerException != null)
                                {
                                    sb.AppendLine(string.Format("InnerException Type: {0}", exception.InnerException.GetType().FullName.ToString()));
                                    sb.AppendLine(string.Format("InnerException Message: {0}", exception.InnerException.Message));
                                    sb.AppendLine(string.Format("InnerException StackTrace: {0}", exception.InnerException.StackTrace.ToString()));
                                }
                                sb.AppendLine("------ Exception End ------");

                                writer.Write(sb.ToString());
                            }
                        }
                    }
                }
            }
            catch (IOException)
            {
                // ログ出力できないだけなので、握りつぶす。
                return;
            }
        }

        public static void Debug(string message, Exception exception = null)
        {
            Write(LogLevel.Level99, message, exception);
        }

        public static void Debug(string message, params object[] args)
        {
            Write(LogLevel.Level99, string.Format(message, args), null);
        }

        public static void Debug(Exception exception)
        {
            Write(LogLevel.Level99, null, exception);
        }

        public static void Error(string message, Exception exception = null)
        {
            Write(LogLevel.Level1, message, exception);
        }

        public static void Error(Exception exception)
        {
            Write(LogLevel.Level1, null, exception);
        }

        #endregion
    }
}
