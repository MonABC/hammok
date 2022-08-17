using System.IO;

public static class Config
{
    //public static readonly string API_BASE_ADDRESS = "https://takahisa-ishikawa-relayapp01-web-apim.azure-api.net";
    public static readonly string API_BASE_ADDRESS = "https://localhost:7000/";
    public static readonly string API_GET_LASTEST_VERSION_REQUEST_URI = "api/autoupdate/v1/getLatestVersion";
    public static readonly string API_DOWNLOAD_LASTEST_VERSION_REQUEST_URI = "api/autoupdate/v1/downloadLatestVersion";

    public static readonly string PARENT_FOLDER = Path.Combine(Path.GetTempPath(), "astAutoUpdate");

    public static readonly string SOFTWARE_NAME = "ast_CAutoUpdate";
    public static readonly string AST_CAUTOUPDATE_FOLDER_PATH = Path.Combine(PARENT_FOLDER, SOFTWARE_NAME);

    public static readonly string COMPLETED_FILE_NAME = "completed.txt";
    public static readonly string COMPLETED_FILE_PATH = Path.Combine(AST_CAUTOUPDATE_FOLDER_PATH, COMPLETED_FILE_NAME);

    public static readonly string INSTALLER_FILE_NAME = "ast_CAutoUpdateInstaller";
    public static readonly string INSTALLER_FILE_PATH = Path.Combine(AST_CAUTOUPDATE_FOLDER_PATH, INSTALLER_FILE_NAME);

    public static readonly string UNZIP_PASSWORD = "h7HeYNLxmF6WKUhM";
}
