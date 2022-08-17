using Microsoft.Win32;
using NLog;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.Utils
{
    public static class Logger
    {
        private enum Level : int
        {
            Error = 1,
            Warn = 2,
            Debug = 99
        }

        private static readonly Level _level = Level.Debug;
        private static readonly NLog.Logger logger = LogManager.GetLogger("*");
        static Logger()
        {
            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (var subKey = baseKey.OpenSubKey(@"SOFTWARE\Hammock\Utility\AutoUpdate", false))
                {
                    if (subKey != null)
                    {
                        _level = (Level)Convert.ToInt32(subKey.GetValue("LogLevel", "1"));
                        return;
                    }
                }
            }

            _level = Level.Debug;
        }

        public static void StartMethod(
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (_level < Level.Debug)
            {
                return;
            }

            string logMessage = CreateMessage("Start method", "DEBUG", null, callerMemberName, callerFilePath, callerLineNumber);
            logger.Debug(logMessage);
        }

        public static void EndMethod(
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (_level < Level.Debug)
            {
                return;
            }

            string logMessage = CreateMessage("End method", "DEBUG", null, callerMemberName, callerFilePath, callerLineNumber);
            logger.Debug(logMessage);
        }

        public static void Error(
            string message, Exception exception = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (_level < Level.Error)
            {
                return;
            }

            string logMessage = CreateMessage(message, "ERROR", exception, callerMemberName, callerFilePath, callerLineNumber);
            logger.Error(logMessage);
        }

        public static void Error(
            Exception exception,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Error(null, exception, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Debug(
            string message, Exception exception = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (_level < Level.Debug)
            {
                return;
            }

            string logMessage = CreateMessage(message, "DEBUG", exception, callerMemberName, callerFilePath, callerLineNumber);
            logger.Debug(logMessage);
        }

        public static void Debug(
            Exception exception,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Debug(null, exception, callerMemberName, callerFilePath, callerLineNumber);
        }

        private static string CreateMessage(
            string message,
            string label,
            Exception exception,
            string callerMemberName,
            string callerFilePath,
            int callerLineNumber)
        {
            StringBuilder stringBuilder = new StringBuilder();
            _ = stringBuilder.Append('[');
            _ = stringBuilder.Append(Path.GetFileName(callerFilePath));
            _ = stringBuilder.Append(':');
            _ = stringBuilder.Append(callerLineNumber);
            _ = stringBuilder.Append(']');
            _ = stringBuilder.Append(callerMemberName);
            _ = stringBuilder.Append(" ");
            _ = stringBuilder.Append(label.PadRight(5));
            _ = stringBuilder.Append(" ");
            _ = stringBuilder.Append(" : ");
            if (string.IsNullOrEmpty(message) == false)
            {
                _ = stringBuilder.Append(message);
            }
            if (exception != null)
            {
                _ = stringBuilder.Append(exception.ToString());
            }
            return stringBuilder.ToString();
        }
    }
}