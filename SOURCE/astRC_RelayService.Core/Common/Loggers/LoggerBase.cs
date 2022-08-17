using System;
using System.IO;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common.Loggers
{
    public abstract class LoggerBase
    {
        public const string RootLogDirectory = @"C:\ProgramData\Hammock\PLATINUM\Server\Logs\RelayServer\";

        public Func<DateTime> GetToday { get; set; } = () => DateTime.Today;

        /// <summary>
        /// ログファイル名の日付分を除いたもの。
        /// 例えば、本プロパティ値が ABCの場合、ログファイル名はABC_yyyyMMdd.csvとなる。
        /// </summary>
        public abstract string FileNameWithoutDate { get; }

        /// <summary>
        /// ログディレクトリは月ごとに生成し、ログファイルは日ごとに生成するため、
        /// ログファイルパスは都度取得する必要がある。
        /// </summary>
        /// <returns></returns>
        protected string GetCurrentLogFilePath()
        {
            var today = GetToday();
            var directory = Path.Combine(RootLogDirectory, today.Year.ToString("D4"), today.Month.ToString("D2"));
            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }

            var fileName = string.Format(
                "{0}_{1}{2}{3}.csv",
                FileNameWithoutDate,
                today.Year.ToString("D4"),
                today.Month.ToString("D2"),
                today.Day.ToString("D2"));

            var filePath = Path.Combine(directory, fileName);
            return filePath;
        }
    }
}