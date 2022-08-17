using System.IO;

namespace Hammock.AssetView.Platinum.Tools
{
    public static class AutoUpdateSettings
    {
# if DEBUG
        public static readonly string API_BASE_ADDRESS = "https://localhost:7000/";
#else
        public static readonly string API_BASE_ADDRESS = "https://takahisa-ishikawa-rc-web02.azurewebsites.net/";
#endif

        public static readonly string API_GET_LASTEST_VERSION_REQUEST_URI = "api/autoupdate/v1/getLatestVersion";
        public static readonly string API_DOWNLOAD_LASTEST_VERSION_REQUEST_URI = "api/autoupdate/v1/downloadLatestVersion";

        public static readonly string API_USERNAME = "astAutoUpdate";
        public static string API_PASSWORD { get; set; } = "De7S6aHUG3hJjsZW";

        public static readonly string UNZIP_PASSWORD = "h7HeYNLxmF6WKUhM";

        public static readonly string PARENT_FOLDER = Path.Combine(Path.GetTempPath(), "astAutoUpdate");
        public static readonly string COMPLETED_FILE_NAME = "completed.txt";

    }
}