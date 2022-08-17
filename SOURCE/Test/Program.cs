using Azure.Storage.Blobs;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Test
{
    internal class Program
    {
        private static readonly string PARENT_FOLDER = Path.Combine(Path.GetTempPath(), "astAutoUpdate");

        private static readonly string SERVICE_NAME = "ast_CAutoUpdate";
        private static readonly string AST_CAUTOUPDATE_FOLDER_PATH = Path.Combine(PARENT_FOLDER, SERVICE_NAME);

        private static readonly string COMPLETED_FILE_NAME = "completed.txt";
        private static readonly string COMPLETED_FILE_PATH = Path.Combine(AST_CAUTOUPDATE_FOLDER_PATH, COMPLETED_FILE_NAME);

        private static readonly string INSTALLER_FILE_NAME = "ast_CAutoUpdateInstaller";
        private static readonly string INSTALLER_FILE_PATH = Path.Combine(AST_CAUTOUPDATE_FOLDER_PATH, INSTALLER_FILE_NAME);

        private static readonly string API_GET_LASTEST_VERSION_URL = "https://takahisa-ishikawa-relayapp01-web-apim.azure-api.net/api/autoupdate/v1/getLatestVersion";
        private static readonly string API_DOWNLOAD_LASTEST_VERSION_URL = "";

        private static string UNZIP_PASSWORD = "h7HeYNLxmF6WKUhM";

        // アップデート対象のソフトウェア一覧ファイルのパース
        private static string SOFTWARES_JSON_PATH;
        // 直近の自動アップデートサーバーへの問い合わせ情報ファイルのパース
        private static string UPDATE_INQUIRY_INFO_JSON_PATH;

        static void Main(string[] args)
        {
            //Console.WriteLine("//==================================START==================================//");
            //// C:\ProgramData
            //Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

            //string path = "PROGRAMDATA";
            //Console.WriteLine($"%{path}% :\t" + Environment.ExpandEnvironmentVariables($"%{path}%"));
            //// %PROGRAMDATA% : C:\ProgramData

            ////==================================================//

            //// C:\Users\DDVIET\AppData\Roaming
            //Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

            //path = "AppData";
            //Console.WriteLine($"%{path}% :\t" + Environment.ExpandEnvironmentVariables($"%{path}%"));
            //// %AppData% :     C:\Users\DDVIET\AppData\Roaming

            ////==================================================//
            //// C:\Users\DDVIET\AppData\Local\Temp\
            //Console.WriteLine(System.IO.Path.GetTempPath());

            //path = "temp";
            //Console.WriteLine($"%{path}% :\t" + Environment.ExpandEnvironmentVariables($"%{path}%"));
            //// %temp% :        C:\Users\DDVIET\AppData\Local\Temp
            ///
            //Console.WriteLine("//===================================END===================================//");


            //Console.WriteLine("//==================================START==================================//");
            //string strPath = @"C:\ProgramData\Hammock\PLATINUM\Client\AutoUpdate";

            //if (!Directory.Exists(strPath))
            //{
            //    Directory.CreateDirectory(strPath);
            //}
            //Console.WriteLine("//===================================END===================================//");


            //Console.WriteLine("//==================================START==================================//");
            //Console.WriteLine("Start service");
            //ServiceController service = new ServiceController("ast_CAutoUpdate");
            //service.Start();
            //Console.WriteLine("//===================================END===================================//");


            //Console.WriteLine("//==================================START==================================//");
            //Console.WriteLine("Stop service");
            //ServiceController service = new ServiceController("ast_CAutoUpdate");
            //service.Stop();
            //Console.WriteLine("//===================================END===================================//");


            //Console.WriteLine("//==================================START==================================//");
            //Console.WriteLine("Unzip file with password");
            //using (var archive = new SevenZipArchive("Sample_Encrypted.7z"))
            //{
            //    // Extract or open 7z archive with password.
            //    archive.ExtractToDirectory("Sample_Encrypted7zip", "aspose");
            //}
            //using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read("test_unzip_aes256.zip"))
            //{
            //    zip.Password = UNZIP_PASSWORD;
            //    zip.ExtractAll(AppDomain.CurrentDomain.BaseDirectory, Ionic.Zip.ExtractExistingFileAction.DoNotOverwrite);
            //}
            //Console.WriteLine("//===================================END===================================//");


            //Console.WriteLine("//==================================START==================================//");
            //private readonly string USER_NAME = "astAutoUpdate";
            //private readonly string PASSWORD = "De7S6aHUG3hJjsZW";
            //Console.WriteLine("//===================================END===================================//");


            //Console.WriteLine("//==================================START==================================//");
            //Console.WriteLine(@"Write registry HKEY_LOCAL_MACHINE\SOFTWARE\Hammock\Utility\AutoUpdate");
            //using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            //{
            //    using (var key = hklm.CreateSubKey(@"SOFTWARE\Hammock\Utility\AutoUpdate", writable: true))
            //    {
            //        if (key.GetValue("ProcessMonitorInterval") == null)
            //        {
            //            key.SetValue("ProcessMonitorInterval", "3");
            //        }

            //        if (key.GetValue("WebServiceInquiryInterval") == null)
            //        {
            //            key.SetValue("WebServiceInquiryInterval", "180");
            //        }

            //        if (key.GetValue("WebServiceRetryInterval") == null)
            //        {
            //            key.SetValue("WebServiceRetryInterval", "60");
            //        }

            //        if (key.GetValue("WebServiceRetryCount") == null)
            //        {
            //            key.SetValue("WebServiceRetryCount", "3");
            //        }

            //        if (key.GetValue("LogLevel") == null)
            //        {
            //            key.SetValue("LogLevel", "1");
            //        }
            //    }
            //}
            //Console.WriteLine("//===================================END===================================//");

            //Console.WriteLine("//==================================START==================================//");
            //Console.WriteLine(@"Delete registry HKEY_LOCAL_MACHINE\SOFTWARE\Hammock\Utility\AutoUpdate");
            //using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            //{
            //    using (var key = hklm.OpenSubKey(@"SOFTWARE\Hammock\Utility\AutoUpdate", writable: true))
            //    {
            //        if (key != null)
            //        {
            //            key.DeleteValue("ProcessMonitorInterval");
            //            key.DeleteValue("WebServiceInquiryInterval");
            //            key.DeleteValue("WebServiceRetryInterval");
            //            key.DeleteValue("WebServiceRetryCount");
            //            key.DeleteValue("LogLevel");
            //        }
            //    }
            //}
            //Console.WriteLine("//===================================END===================================//");


            //Console.WriteLine("//==================================START==================================//");
            //string strFilePath = @"D:\DU2\hammock\ast_CAutoUpdateInstaller\Debug\ast_CAutoUpdateInstaller.msi";

            //System.Diagnostics.Process process = new System.Diagnostics.Process();
            //System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //startInfo.FileName = "msiexec";
            //startInfo.Arguments = "/i " + strFilePath;
            //process.StartInfo = startInfo;
            //process.Start();
            //process.WaitForExit();

            //string strCmdText;
            //strCmdText = @"D:\DU2\hammock\ast_CAutoUpdateInstaller\Debug\ast_CAutoUpdateInstaller.msi";
            //System.Diagnostics.Process.Start("CMD.exe", strCmdText);

            //Process process3 = new Process();
            //process3.StartInfo.FileName = "cmd.exe";
            ////process.StartInfo.Arguments = "/i " + strFilePath;
            //process3.StartInfo.RedirectStandardInput = true;
            //process3.StartInfo.RedirectStandardOutput = true;
            //process3.StartInfo.CreateNoWindow = true;
            //process3.StartInfo.UseShellExecute = false;
            //process3.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //process3.Start();

            //process3.StandardInput.WriteLine("msiexec /i " + strFilePath);
            //process3.StandardInput.Flush();
            //process3.StandardInput.Close();
            //process3.WaitForExit();
            //Console.WriteLine("//===================================END===================================//");

            //string strPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Hammock\PLATINUM\Utility\AutoUpdateTool\ast_CAutoUpdate.exe");
            ////strPath = @"D:\Test\ast_CAutoUpdate.exe";
            //strPath = "\"" + strPath + "\"";

            //System.Diagnostics.Process process6 = new System.Diagnostics.Process();
            //System.Diagnostics.ProcessStartInfo startInfo6 = new System.Diagnostics.ProcessStartInfo();
            //startInfo6.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //startInfo6.FileName = @"c:\windows\microsoft.net\framework\v4.0.30319\installutil.exe";
            //startInfo6.Arguments = "/u " + strPath;
            //process6.StartInfo = startInfo6;
            //process6.Start();
            //process6.WaitForExit();

            //System.Diagnostics.Process process4 = new System.Diagnostics.Process();
            //System.Diagnostics.ProcessStartInfo startInfo4 = new System.Diagnostics.ProcessStartInfo();
            //startInfo4.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //startInfo4.FileName = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe";
            //startInfo4.Arguments = strPath;
            //process4.StartInfo = startInfo4;
            //process4.Start();
            //process4.WaitForExit();

            //Process process5 = new Process();
            //process5.StartInfo.FileName = "sc";
            //process5.StartInfo.Arguments = "failure \"ast_CAutoUpdate\" reset= 300 command= \"\" actions= restart/////";
            ////process5.StartInfo.RedirectStandardInput = true;
            ////process5.StartInfo.RedirectStandardOutput = true;
            //process5.StartInfo.CreateNoWindow = true;
            //process5.StartInfo.UseShellExecute = false;
            //process5.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //process5.Start();

            ////process5.StandardInput.WriteLine("msiexec /i " + strFilePath);
            ////process5.StandardInput.Flush();
            ////process5.StandardInput.Close();
            //process5.WaitForExit();

            //Console.WriteLine("//==================================START==================================//");
            //Console.WriteLine("Azure_ReadFileContent : SoftwareName = ast_CAutoUpdate");
            //Azure_ReadFileContent("ast_CAutoUpdate");
            //Console.WriteLine("Azure_ReadFileContent : SoftwareName = astRC_RelayClient");
            //Azure_ReadFileContent("astRC_RelayClient");
            //Console.WriteLine("//===================================END===================================//");


            //Console.WriteLine("//==================================START==================================//");
            //Console.WriteLine("GetNewestVersion : SoftwareName = ast_CAutoUpdate");
            //GetNewestVersion("ast_CAutoUpdate");
            //Console.WriteLine("GetNewestVersion : SoftwareName = astRC_RelayClient");
            //GetNewestVersion("astRC_RelayClient");
            //Console.WriteLine("//===================================END===================================//");


            //Console.WriteLine("//==================================START==================================//");
            //string strData = "[ \"ast_CAutoUpdate\", \"astRC_RelayClient\" ]";
            //List<string> lstData = JsonConvert.DeserializeObject<IEnumerable<string>>(strData).ToList();

            //// Softwares.json
            //// C:\ProgramData\Hammock\PLATINUM\Client\AutoUpdate\Softwares.json
            //SOFTWARES_JSON_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Hammock\PLATINUM\Client\AutoUpdate\Softwares.json");

            //// UpdateInquiryInfo.json
            //// %AppData%\Hammock\AssetView\AutoUpdate\UpdateInquiryInfo.json
            //UPDATE_INQUIRY_INFO_JSON_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Hammock\AssetView\AutoUpdate\UpdateInquiryInfo.json");


            //// アップデート対象のソフトウェア一覧を読み込む
            //List<string> lstSoftwareName = new List<string>();

            //// C:\ProgramData\Hammock\PLATINUM\Client\AutoUpdate\Softwares.json
            //if (File.Exists(SOFTWARES_JSON_PATH))
            //{
            //    // 例：[ "ast_CAutoUpdate", "astRC_RelayClient" ]
            //    string strData = File.ReadAllText(SOFTWARES_JSON_PATH);

            //    // 例：
            //    //      ast_CAutoUpdate
            //    //      astRC_RelayClient
            //    lstSoftwareName = JsonConvert.DeserializeObject<IEnumerable<string>>(strData).ToList();
            //}

            //// 設定ファイルを読み込み、前回のサーバー問い合わせ日時を確認する
            //List<UpdateInquiryInfo> lstUpdateInquiryInfo = new List<UpdateInquiryInfo>();

            //// C:\Users\DoDucViet\AppData\Roaming\Hammock\AssetView\AutoUpdate\UpdateInquiryInfo.json
            //if (File.Exists(UPDATE_INQUIRY_INFO_JSON_PATH))
            //{
            //    // 例：
            //    // [{
            //    //    SoftwareName:"programX",
            //    //    LastTimeOfInquiryTime:"2022/07/01 12:34:56",
            //    // },{
            //    //    SoftwareName:"programY",
            //    //    LastTimeOfInquiryTime:"2022/07/21 12:34:56",
            //    // }]
            //    string strData = File.ReadAllText(UPDATE_INQUIRY_INFO_JSON_PATH);
            //    lstUpdateInquiryInfo = JsonConvert.DeserializeObject<IEnumerable<UpdateInquiryInfo>>(strData).ToList();
            //}
            //Console.WriteLine("//===================================END===================================//");


            //string currenttProgramName = AppDomain.CurrentDomain.FriendlyName;

            string strProcessFilePath = Process.GetCurrentProcess().MainModule.FileName;
            Console.WriteLine("strProcessFilePath : " + strProcessFilePath);
            string strDestFolder = new FileInfo(Process.GetCurrentProcess().MainModule.FileName).Directory.FullName;
            Console.WriteLine("strDestFolder : " + strDestFolder);








            //Console.WriteLine("//==================================START==================================//");
            //Console.WriteLine("DownloadFile_GetMethod");
            //DownloadFile_GetMethod();
            //Console.WriteLine("//===================================END===================================//");


            //Console.WriteLine("//==================================START==================================//");
            //Console.WriteLine("DownloadFile_PostMethod");
            //DownloadFile_PostMethod("ast_CAutoUpdate", "1.1.1.1");
            //Console.WriteLine("//===================================END===================================//");







            Console.WriteLine("!!! END OF PROGRAM !!!");
            Console.ReadLine();
        }

        private static void Azure_ReadFileVersionContent(string SoftwareName)
        {
            //BlobServiceClient blobServiceClient = new BlobServiceClient(API.CONNECTION_STRING);
            //BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(API.BLOB_CONTAINER_NAME);
            //BlobClient blobClient = containerClient.GetBlobClient(string.Format("{0}/{1}", model.SoftwareName, "version.txt"));

            string blobName = string.Format("{0}/{1}", SoftwareName, "version.txt");

            BlobClient blobClient = new BlobClient(API.CONNECTION_STRING, API.BLOB_CONTAINER_NAME, blobName);

            if (blobClient.Exists())
            {
                var response = blobClient.Download();

                using (var streamReader = new StreamReader(response.Value.Content))
                {
                    while (!streamReader.EndOfStream)
                    {
                        var line = streamReader.ReadLineAsync();
                        Console.WriteLine(line.Result);
                    }
                }
            }
        }

        public class AutoUpdate_GetNewest
        {
            public string SoftwareName { get; set; }
        }

        private static async void GetNewestVersion(string softwareName)
        {
            using (HttpClient client = new HttpClient())
            {
                AutoUpdate_GetNewest p = new AutoUpdate_GetNewest { SoftwareName = softwareName };
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json");

                client.BaseAddress = new Uri("https://localhost:7000/");
                HttpResponseMessage response = client.PostAsync("api/autoupdate/v1/getLatestVersion", stringContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    //Console.WriteLine("Success");
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
        }

        private static void DownloadFile_GetMethod()
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/zip";

                byte[] r = client.DownloadData("https://localhost:7000/api/autoupdate/v1/downloadLatestVersion2");
                using (var stream = System.IO.File.Create(Path.Combine(AST_CAUTOUPDATE_FOLDER_PATH, "installer.zip")))
                {
                    stream.Write(r, 0, r.Length);
                }
            }

        }

        public class AutoUpdate_DownloadLastest
        {
            public string SoftwareName { get; set; }
            public string Version { get; set; }
        }

        private static async void DownloadFile_PostMethod(string softwareName, string version)
        {
            using (HttpClient client = new HttpClient())
            {
                AutoUpdate_DownloadLastest autoUpdate = new AutoUpdate_DownloadLastest { SoftwareName = softwareName, Version = version };
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(autoUpdate), Encoding.UTF8, "application/json");

                client.BaseAddress = new Uri(Config.API_BASE_ADDRESS);
                HttpResponseMessage response = client.PostAsync(Config.API_DOWNLOAD_LASTEST_VERSION_REQUEST_URI, stringContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    byte[] b = await response.Content.ReadAsByteArrayAsync();

                    using (var stream = System.IO.File.Create(Path.Combine(Config.AST_CAUTOUPDATE_FOLDER_PATH, "ast_CAutoUpdateInstaller.zip")))
                    {
                        stream.Write(b, 0, b.Length);
                    }
                }
            }

        }
    }
}