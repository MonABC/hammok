using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.ServiceProcess;
using System.Diagnostics;
using Hammock.AssetView.Platinum.Tools.Properties;
using Hammock.AssetView.Platinum.Tools.Utils;
using System.Reflection;
using CommandLine;
using System.Collections;
using System.Collections.Generic;

namespace Hammock.AssetView.Platinum.Tools
{
    internal class Program
    {
        private static readonly string processId = Guid.NewGuid().ToString();

        static void Main(string[] args)
        {
            try
            {
                Logger.Debug($"{Assembly.GetEntryAssembly().FullName}を開始しします。Id={processId}");

                //#if !DEBUG
                Parser.Default.ParseArguments<Args>(args)
                    .WithParsed<Args>(o1 => WithParsed(o1))
                    .WithNotParsed<Args>(o2 => WithNotParsed(o2));
                //#endif

                string strSoftwareName = "ast_CAutoUpdate";
                string strWorkingFolder = Path.Combine(AutoUpdateSettings.PARENT_FOLDER, strSoftwareName);
                string strCompletedFilePath = Path.Combine(strWorkingFolder, AutoUpdateSettings.COMPLETED_FILE_NAME);
                string strInstallerFilePath = Path.Combine(strWorkingFolder, strSoftwareName + "Installer.msi");
                string strZipFilePath = Path.Combine(strWorkingFolder, strSoftwareName + "Installer.zip");

                string strCurrentVersion = string.Empty;
                string strNewestVersion = APIUtils.GetNewestVersion(strSoftwareName).Result;

                if (File.Exists(strCompletedFilePath))
                {
                    strCurrentVersion = File.ReadAllText(strCompletedFilePath);
                }

                if (string.IsNullOrEmpty(strCurrentVersion)
                    || strCurrentVersion != strNewestVersion
                   )
                {

                    // 引き続きダウンロード処理を行う

                    FileUtils.DeleteFile(strInstallerFilePath);
                    APIUtils.DownloadNewestInstallerFile(strSoftwareName, strNewestVersion).Wait();
                    ZipUtils.UnzipFile(strWorkingFolder, strZipFilePath, AutoUpdateSettings.UNZIP_PASSWORD);
                    AutoUpdateUtils.CreateCompletedFile(strWorkingFolder, strNewestVersion);
                    WindowServiceUtils.StopService(strSoftwareName);
                    RunInstaller(strInstallerFilePath);
                    WindowServiceUtils.StartService(strSoftwareName);
                    FileUtils.DeleteFiles(strWorkingFolder, new List<string>() { AutoUpdateSettings.COMPLETED_FILE_NAME });
                }
                else
                {
                    return;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.AnUnexpectedErrorOccurred, "AssetView セルフアップデート", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Error($"不明な例外が発生しました。Id={processId}, Exception={ex.ToString()}");
            }
            finally
            {
                Logger.Debug($"{Assembly.GetEntryAssembly().FullName}を終了しました。Id={processId}");
            }
        }

        private static void WithParsed(Args args)
        {
            if (args.Check != "{CE3405EA-537E-4F61-95BE-6716B27EA5C2}")
            {
                throw new InvalidOperationException();
            }
        }

        private static void WithNotParsed(IEnumerable errs)
        {
            throw new InvalidOperationException();
        }

        private static void RunInstaller(string strFilePath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "msiexec";
            process.StartInfo.Arguments = "/i " + strFilePath;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }
    }
}