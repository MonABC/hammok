namespace Hammock.AssetView.Platinum.Tools.Utils
{
    internal class ZipUtils
    {
        internal static void UnzipFile(string strDestFolderPath, string strZipFilePath, string strPassword)
        {
            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(strZipFilePath))
            {
                zip.Password = strPassword;
                zip.ExtractAll(strDestFolderPath, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}