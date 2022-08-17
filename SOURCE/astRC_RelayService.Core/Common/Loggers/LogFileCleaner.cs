using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common.Loggers
{
    /// <summary>
    /// 古くなったログファイルの削除を行う。
    /// </summary>
    public class LogFileCleaner
    {
        private readonly double _logCleanerInterval;
        private readonly double _logSaveDays;
        private readonly object sync = new object();
        private DateTime _nextCleanTime;
        public LogFileCleaner()
        {
            _logCleanerInterval = double.Parse(ConfigurationManager.AppSettings["LogCleanerInterval"]);
            _logSaveDays = double.Parse(ConfigurationManager.AppSettings["LogSaveDays"]);
            _nextCleanTime = DateTime.Now.AddMinutes(-1);

            Logger.Debug("ログ削除間隔={0}時間, ログ保存日数={1}日", _logCleanerInterval, _logSaveDays);
        }

        public void Clean()
        {
            if (_nextCleanTime > DateTime.Now)
            {
                return;
            }

            lock (sync)
            {
                if (_nextCleanTime > DateTime.Now)
                {
                    return;
                }

                _nextCleanTime = DateTime.Now.AddHours(_logCleanerInterval);
                CleanInternal();
            }
        }

        private void CleanInternal()
        {
            var rootDirectory = LoggerBase.RootLogDirectory;
            if (Directory.Exists(rootDirectory) == false)
            {
                return;
            }

            var today = DateTime.Today;
            foreach (var yearDirectory in Directory.GetDirectories(rootDirectory))
            {
                var name = Path.GetFileName(yearDirectory);
                if (int.TryParse(name, out var year) == false)
                {
                    continue;
                }

                CleanYearDirectory(yearDirectory, year, today);
                DeleteDirectoryIfEmpty(yearDirectory);
            }
        }

        private void CleanYearDirectory(string yearDirectory, int year, DateTime today)
        {
            foreach (var monthDirectory in Directory.GetDirectories(yearDirectory))
            {
                var name = Path.GetFileName(monthDirectory);
                if (int.TryParse(name, out var month) == false)
                {
                    continue;
                }
                if (month <= 0 || month >= 13)
                {
                    continue;
                }

                CleanMonthDirectory(year, month, monthDirectory, today);
                DeleteDirectoryIfEmpty(monthDirectory);
            }
        }

        private static void DeleteDirectoryIfEmpty(string directory)
        {
            if (Directory.GetFileSystemEntries(directory).Length == 0)
            {
                try
                {
                    Directory.Delete(directory, true);
                }
                catch (IOException ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        private void CleanMonthDirectory(int year, int month, string monthDirectory, DateTime today)
        {
            foreach (var logFile in Directory.GetFiles(monthDirectory, "*.csv"))
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(logFile);
                int lastIndex = fileNameWithoutExtension.LastIndexOf('_');
                if (lastIndex < 0)
                {
                    continue;
                }

                var yearText = year.ToString("D4");
                var yearMonthDay = fileNameWithoutExtension.Substring(1 + lastIndex);
                if (yearMonthDay.StartsWith(yearText) == false)
                {
                    continue;
                }

                var monthDay = yearMonthDay.Substring(yearText.Length);
                var monthText = month.ToString("D2");
                if (monthDay.StartsWith(monthText) == false)
                {
                    continue;
                }
                var dayText = monthDay.Substring(monthText.Length);
                if (int.TryParse(dayText, out var day) == false)
                {
                    continue;
                }

                var logDate = new DateTime(year, month, day);
                var diff = today - logDate;
                if (diff.TotalDays > _logSaveDays)
                {
                    try
                    {
                        File.Delete(logFile);
                    }
                    catch (IOException ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }
        }
    }
}

