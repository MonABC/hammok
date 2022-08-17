using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    public static class ProgramLogWriter
    {
        private static string DirectoryName { get; set; }
        private static string FileBaseName { get; set; }
        private const int MaxFiles = 10;
        private const int MaxFilesize = 1*1024*1024;
        private static bool WroteVersion { get; set; }
        private static readonly Object MThisLock = new Object();

        static ProgramLogWriter()
        {
        }

        public static void Init(string sDirectory, string sFileBaseName)
        {
            DirectoryName = sDirectory;
            FileBaseName = sFileBaseName;
            if (!Directory.Exists(DirectoryName))
            {
                Directory.CreateDirectory(DirectoryName);
            }
        }

        public static void Init(Environment.SpecialFolder sf, string sDirectory, string sFileBaseName)
        {
            DirectoryName = Path.Combine(Environment.GetFolderPath(sf), sDirectory);
            FileBaseName = sFileBaseName;
        }

        private static string GetModuleVersion()
        {
            var sLocation = Assembly.GetExecutingAssembly().Location;
            return String.Format("Version:({0}) ({1})", FileVersionInfo.GetVersionInfo(sLocation).FileVersion, sLocation);
        }

        private static void RotateFile(string sLogFileBase)
        {
            long lFileSize;
            using (var fs = new FileStream(string.Format("{0}_01.log", sLogFileBase), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                lFileSize = fs.Length;
            }
            if (lFileSize >= (Variables.LogLevel < 99 ? MaxFilesize : MaxFilesize*10))
            {
                for (var i = MaxFiles; i >= 1; i--)
                {
                    var sCurrentLogFile = String.Format("{0}_{1:00}.log", sLogFileBase, i);
                    var sNewLogFile = String.Format("{0}_{1:00}.log", sLogFileBase, i + 1);
                    if (File.Exists(sCurrentLogFile))
                    {
                        File.Move(sCurrentLogFile, sNewLogFile);
                        if (i == MaxFiles)
                            File.Delete(sNewLogFile);
                    }
                }
            }
            if (!WroteVersion && (!File.Exists(string.Format("{0}_01.log", sLogFileBase)) || lFileSize == 0))
            {
                using (var fs = new FileStream(string.Format("{0}_01.log", sLogFileBase), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    using (var sw = new StreamWriter(fs, Encoding.Unicode))
                    {
                        var sDateTime = String.Format("{0}.{1}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), DateTime.Now.Millisecond.ToString("000"));
                        sw.WriteLine(String.Format("{0}    {1}", sDateTime, GetModuleVersion()));
                    }
                }
            }
        }

        public static void Write(int iLogLevel, string sMessage1)
        {
            try
            {
                if (iLogLevel <= Variables.LogLevel)
                {
                    lock (MThisLock)
                    {
                        var sLogFileBase = Path.Combine(DirectoryName, FileBaseName);
                        RotateFile(sLogFileBase);
                        using (var sw = new StreamWriter(string.Format("{0}_01.log", sLogFileBase), true, Encoding.Unicode))
                        {
                            var sDateTime = String.Format("{0}.{1}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), DateTime.Now.Millisecond.ToString("000"));
                            sw.WriteLine(String.Format("{0}    {1}", sDateTime, sMessage1));
                        }
                    }
                }
            }
            catch (IOException)
            {
                //ログ出力時の例外は握りつぶす。
            }
        }

        public static void Write(int iLogLevel, string sMessage1, string sMessage2)
        {
            try
            {
                if (iLogLevel <= Variables.LogLevel)
                {
                    lock (MThisLock)
                    {
                        var sLogFileBase = Path.Combine(DirectoryName, FileBaseName);
                        RotateFile(sLogFileBase);
                        using (var sw = new StreamWriter(string.Format("{0}_01.log", sLogFileBase), true, Encoding.Unicode))
                        {
                            var sDateTime = String.Format("{0}.{1}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), DateTime.Now.Millisecond.ToString("000"));
                            sw.WriteLine(String.Format("{0}    {1} ({2})", sDateTime, sMessage1, sMessage2));
                        }
                    }
                }
            }
            catch (IOException)
            {
                //ログ出力時の例外は握りつぶす。
            }
        }

        /// <summary>
        ///   ログファイル書き込み
        /// </summary>
        /// <param name = "iLogLevel"></param>
        /// <param name = "exception"></param>
        public static void Write(int iLogLevel, Exception exception)
        {
            try
            {
                Write(iLogLevel, exception, 1);
            }
            catch (IOException)
            {
                //ログ出力時の例外は握りつぶす。
            }
        }

        /// <summary>
        ///   ログファイル書き込み
        /// </summary>
        /// <param name = "iLogLevel"></param>
        /// <param name = "exception"></param>
        /// <param name = "exceptionLevel"></param>
        private static void Write(int iLogLevel, Exception exception, int exceptionLevel)
        {
            Write(iLogLevel, string.Format("EXCEPTION({0}) {1}", exceptionLevel, exception.Message), exception.StackTrace);
            Exception innerException = exception.InnerException;

            //InnerExceptionがある場合は出力する。
            if (innerException != null)
            {
                Write(iLogLevel, innerException, exceptionLevel + 1);
            }
        }

        public static void WriteVersion()
        {
            WroteVersion = true;
            Write(1, GetModuleVersion());
            WroteVersion = false;
        }
    }
}
