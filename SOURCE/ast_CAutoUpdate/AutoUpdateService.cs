using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using Newtonsoft.Json;
using Hammock.AssetView.Platinum.Tools.Utils;
using Hammock.AssetView.Platinum.Tools.Models;
using System.Reflection;

namespace Hammock.AssetView.Platinum.Tools
{
    public partial class AutoUpdateService : ServiceBase
    {
        private readonly string processId = Guid.NewGuid().ToString();

        Timer timer = new Timer();
        List<string> lstSoftwareName;

        private bool alreadyShowErrorMessage;

        private string SOFTWARES_JSON_PATH;
        private string UPDATE_INQUIRY_INFO_JSON_PATH;

        #region レジストリ
        int iProcessMonitorInterval = 3;
        int iWebServiceInquiryInterval = 180;
        int iWebServiceRetryInterval = 60;
        int iWebServiceRetryCount = 3;
        int iLogLevel = 1;
        #endregion レジストリ

        public AutoUpdateService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Debug($"{Assembly.GetEntryAssembly().FullName}を開始しします。Id={processId}");

            System.Diagnostics.Debugger.Launch();
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += (sender, e2) => UnhandledException((Exception)e2.ExceptionObject);

            SOFTWARES_JSON_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Hammock\Utility\AutoUpdate\Softwares.json");
            UPDATE_INQUIRY_INFO_JSON_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Hammock\Utility\AutoUpdate\UpdateInquiryInfo.json");

            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (var subKey = baseKey.OpenSubKey(@"SOFTWARE\Hammock\Utility\AutoUpdate", false))
                {
                    iProcessMonitorInterval = Convert.ToInt32(subKey.GetValue("ProcessMonitorInterval", "3"));
                    iWebServiceInquiryInterval = Convert.ToInt32(subKey.GetValue("WebServiceInquiryInterval", "180"));
                    iWebServiceRetryInterval = Convert.ToInt32(subKey.GetValue("WebServiceRetryInterval", "60"));
                    iWebServiceRetryCount = Convert.ToInt32(subKey.GetValue("WebServiceRetryCount", "3"));
                    iLogLevel = Convert.ToInt32(subKey.GetValue("LogLevel", "1"));
                }
            }

            string strData = File.ReadAllText(SOFTWARES_JSON_PATH);
            lstSoftwareName = JsonConvert.DeserializeObject<IEnumerable<string>>(strData).ToList();

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = iProcessMonitorInterval * 60000;
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            Logger.Debug($"{Assembly.GetEntryAssembly().FullName}を終了しました。Id={processId}");
        }

        private void UnhandledException(Exception ex)
        {
            if (alreadyShowErrorMessage)
            {
                return;
            }

            Logger.Error($"不明な例外が発生しました。Id={processId}, Exception={ex.ToString()}");
            alreadyShowErrorMessage = true;
        }

        private void OnElapsedTime(object source, ElapsedEventArgs ex)
        {
            string strCurrentVersion = "";
            string strNewestVersion = "";

            List<UpdateInquiryInfo> lstUpdateInquiryInfo = ReadUpdateInquiryInfo();

            foreach (string softwareName in lstSoftwareName)
            {
                // アップデート対象ソフトウェアの起動を確認
                var isExistProcess = Process.GetProcessesByName(softwareName).FirstOrDefault();
                if (isExistProcess == null)
                {
                    continue;
                }

                bool isNeedUpdate = false;

                UpdateInquiryInfo updateInquiryInfo;
                DateTime now = DateTime.Now;

                string strWorkingFolder = Path.Combine(AutoUpdateSettings.PARENT_FOLDER, softwareName);
                string strCompletedFilePath = Path.Combine(strWorkingFolder, AutoUpdateSettings.COMPLETED_FILE_NAME);
                string strInstallerFilePath = Path.Combine(strWorkingFolder, softwareName + "Installer.msi");
                string strZipFilePath = Path.Combine(strWorkingFolder, softwareName + "Installer.zip");

                if (!lstUpdateInquiryInfo.Exists(x => x.SoftwareName == softwareName))
                {
                    isNeedUpdate = true;

                    updateInquiryInfo = new UpdateInquiryInfo();
                    updateInquiryInfo.SoftwareName = softwareName;
                    updateInquiryInfo.LastTimeOfInquiryTime = now;
                    lstUpdateInquiryInfo.Add(updateInquiryInfo);
                }
                else
                {
                    updateInquiryInfo = lstUpdateInquiryInfo.Find(x => x.SoftwareName == softwareName);

                    if ((now - updateInquiryInfo.LastTimeOfInquiryTime).TotalMinutes > iWebServiceInquiryInterval)
                    {
                        strNewestVersion = APIUtils.GetNewestVersion(softwareName).Result;
                        updateInquiryInfo.LastTimeOfInquiryTime = now;

                        if (File.Exists(strCompletedFilePath))
                        {
                            strCurrentVersion = File.ReadAllText(strCompletedFilePath);
                        }

                        isNeedUpdate = false;
                        
                        if (strCurrentVersion != strNewestVersion)
                        {
                            isNeedUpdate = true;
                        }
                    }
                }

                SaveUpdateInquiryInfo(lstUpdateInquiryInfo);

                if (isNeedUpdate)
                {
                    if (softwareName == Process.GetCurrentProcess().ProcessName)
                    {
                        SelfUpdate(strNewestVersion);
                    }
                    else
                    {
                        APIUtils.DownloadNewestInstallerFile(softwareName, strNewestVersion).Wait();
                        AutoUpdateUtils.CreateCompletedFile(strWorkingFolder, strNewestVersion);

                        string strDestFolder = "";
                        string strTmpFolder = Path.Combine(strWorkingFolder, "tmp");

                        Process[] processes = Process.GetProcessesByName(softwareName);
                        Process process = processes[0];
                        strDestFolder = new FileInfo(process.MainModule.FileName).Directory.FullName;
                        process.WaitForExit();

                        ZipUtils.UnzipFile(strTmpFolder, strZipFilePath, AutoUpdateSettings.UNZIP_PASSWORD);

                        foreach (var file in Directory.GetFiles(strTmpFolder))
                        {
                            FileInfo fileInfo = new FileInfo(file);

                            File.Copy(file, Path.Combine(strDestFolder, fileInfo.Name), true);
                        }

                        FileUtils.DeleteFile(strZipFilePath);
                        FolderUtils.DeleteFolder(strTmpFolder);
                    }
                }
            }
        }

        private void SaveUpdateInquiryInfo(List<UpdateInquiryInfo> lstUpdateInquiryInfo)
        {
            string strNewData = JsonConvert.SerializeObject(lstUpdateInquiryInfo);
            File.WriteAllText(UPDATE_INQUIRY_INFO_JSON_PATH, strNewData);
        }

        private List<UpdateInquiryInfo> ReadUpdateInquiryInfo()
        {
            if (File.Exists(UPDATE_INQUIRY_INFO_JSON_PATH))
            {
                string strData = File.ReadAllText(UPDATE_INQUIRY_INFO_JSON_PATH);
                return JsonConvert.DeserializeObject<IEnumerable<UpdateInquiryInfo>>(strData).ToList();
            }

            return new List<UpdateInquiryInfo>();
        }

        private void SelfUpdate(string strNewestVersion)
        {
            string strFileName = "ast_CSelfUpdate.exe";
            string strSourceFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Hammock\PLATINUM\Utility\AutoUpdateTool");
            string strDestFolder = Path.Combine(Path.GetTempPath(), @"astAutoUpdate\Self");
            string strSelfFile = Path.Combine(strDestFolder, strFileName);

            FolderUtils.DeleteFolder(strDestFolder);
            FileUtils.CopyFiles(strSourceFolder, strDestFolder);
            StartSelfFile(strSelfFile, strNewestVersion);
        }

        private void StartSelfFile(string strFilePath, string strNewestVersion)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = strFilePath;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = "-v " + strNewestVersion + " -c {CE3405EA-537E-4F61-95BE-6716B27EA5C2}";

            Process.Start(startInfo);
        }
    }
}