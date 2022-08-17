using Hammock.AssetView.Platinum.Tools.Models;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hammock.AssetView.Platinum.Tools.Utils
{
    public class APIUtils
    {
        public static async Task<string> GetNewestVersion(string strSoftwareName)
        {
            if (strSoftwareName is null)
            {
                throw new ArgumentNullException(nameof(strSoftwareName));
            }

            string newestVersion = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                ConfigHttpClient(client);

                GetLatestVersionModel autoUpdate = new GetLatestVersionModel { SoftwareName = strSoftwareName };
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(autoUpdate), Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(AutoUpdateSettings.API_GET_LASTEST_VERSION_REQUEST_URI, stringContent).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }

                if (response.IsSuccessStatusCode)
                {
                    newestVersion = await response.Content.ReadAsStringAsync();
                }
            }

            return newestVersion;
        }

        public static async Task DownloadNewestInstallerFile(string strSoftwareName, string strVersion)
        {
            if (strSoftwareName is null)
            {
                throw new ArgumentNullException(nameof(strSoftwareName));
            }

            if (strVersion is null)
            {
                throw new ArgumentNullException(nameof(strVersion));
            }

            string strInstallerFileName = strSoftwareName + "Installer.zip";
            string strInstallerFilePath = Path.Combine(AutoUpdateSettings.PARENT_FOLDER, strSoftwareName, strInstallerFileName);

            if (!Directory.Exists(Path.Combine(AutoUpdateSettings.PARENT_FOLDER, strSoftwareName)))
            {
                Directory.CreateDirectory(Path.Combine(AutoUpdateSettings.PARENT_FOLDER, strSoftwareName));
            }

            DownloadLatestVersionModel autoUpdate = new DownloadLatestVersionModel { SoftwareName = strSoftwareName, Version = strVersion };
            string json = JsonConvert.SerializeObject(autoUpdate, Formatting.Indented);

            using (HttpClient client = new HttpClient())
            {
                ConfigHttpClient(client);

                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(AutoUpdateSettings.API_DOWNLOAD_LASTEST_VERSION_REQUEST_URI, requestContent);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }

                using (var responseContent = response.Content)
                {
                    using (var responseStream = await responseContent.ReadAsStreamAsync())
                    {
                        using (var fileStream = new FileStream(strInstallerFilePath, FileMode.Create))
                        {
                            responseStream.CopyTo(fileStream);
                        }
                    }
                }
            }
        }

        internal static void ConfigHttpClient(HttpClient client)
        {
            client.BaseAddress = new Uri(AutoUpdateSettings.API_BASE_ADDRESS);
            client.DefaultRequestHeaders.Add("userName", AutoUpdateSettings.API_USERNAME);
            client.DefaultRequestHeaders.Add("password", Encrypt.EncryptToString(AutoUpdateSettings.API_PASSWORD));
        }
    }
}