using System.IO;

namespace Hammock.AssetView.Platinum.Tools.Utils
{
    public class AutoUpdateUtils
    {
        public static void CreateCompletedFile(string strFolderPath, string strVersion)
        {
            if (!Directory.Exists(strFolderPath))
            {
                Directory.CreateDirectory(strFolderPath);
            }

            string strCompletedFilePath = Path.Combine(strFolderPath, AutoUpdateSettings.COMPLETED_FILE_NAME);

            File.WriteAllText(strCompletedFilePath, strVersion);
        }
    }
}