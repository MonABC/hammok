using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace Hammock.AssetView.Platinum.Tools
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        private string INSTALLUTIL_PATH = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe";
        private string SERVICE_PATH = string.Empty;
        private string SERVICE_NAME = "ast_CAutoUpdate";

        public Installer()
        {
            InitializeComponent();

            SERVICE_PATH = "\"" + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Hammock\PLATINUM\Utility\AutoUpdateTool\", SERVICE_NAME + ".exe") + "\"";
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            RemoveRegistry();
            RemoveService();
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);

            AddRegistry();
            AddAndConfigService();
        }

        private void AddRegistry()
        {
            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (var key = hklm.CreateSubKey(@"SOFTWARE\Hammock\Utility\AutoUpdate", writable: true))
                {
                    if (key.GetValue("ProcessMonitorInterval") == null)
                    {
                        key.SetValue("ProcessMonitorInterval", "3");
                    }

                    if (key.GetValue("WebServiceInquiryInterval") == null)
                    {
                        key.SetValue("WebServiceInquiryInterval", "180");
                    }

                    if (key.GetValue("WebServiceRetryInterval") == null)
                    {
                        key.SetValue("WebServiceRetryInterval", "60");
                    }

                    if (key.GetValue("WebServiceRetryCount") == null)
                    {
                        key.SetValue("WebServiceRetryCount", "3");
                    }

                    if (key.GetValue("LogLevel") == null)
                    {
                        key.SetValue("LogLevel", "1");
                    }
                }
            }
        }

        private void RemoveRegistry()
        {
            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (var key = hklm.OpenSubKey(@"SOFTWARE\Hammock\Utility\AutoUpdate", writable: true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("ProcessMonitorInterval");
                        key.DeleteValue("WebServiceInquiryInterval");
                        key.DeleteValue("WebServiceRetryInterval");
                        key.DeleteValue("WebServiceRetryCount");
                        key.DeleteValue("LogLevel");
                    }
                }
            }
        }

        private void AddAndConfigService()
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = INSTALLUTIL_PATH;
            process.StartInfo.Arguments = SERVICE_PATH;
            process.Start();
            process.WaitForExit();

            process = new Process();
            process.StartInfo = new ProcessStartInfo();
            process.StartInfo.FileName = "sc";
            process.StartInfo.Arguments = "failure \"ast_CAutoUpdate\" reset=300 command= \"\" actions= restart/////";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
        }

        private void RemoveService()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = INSTALLUTIL_PATH;
            startInfo.Arguments = "/u " + SERVICE_PATH;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}