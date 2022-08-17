using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hammock.AssetView.Platinum.Tools.Utils
{
    internal class FileUtils
    {
        internal static void UnzipFile(string strDestFolderPath, string strZipFilePath, string strPassword)
        {
            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(strZipFilePath))
            {
                zip.Password = strPassword;
                zip.ExtractAll(strDestFolderPath, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
            }
        }

        internal static List<string> GetAllFiles(string strFolder, bool isIncludeSubDirectories = false)
        {
            if (!Directory.Exists(strFolder))
            {
                return new List<string>();
            }

            SearchOption searchOption = SearchOption.TopDirectoryOnly;

            if (isIncludeSubDirectories)
            {
                searchOption = SearchOption.AllDirectories;
            }

            return Directory.GetFiles(strFolder, "*.*", searchOption).ToList();
        }

        internal static void CopyFiles(string strSourceFolder, string strDestFolder)
        {
            if (!Directory.Exists(strSourceFolder))
            {
                return;
            }

            if (!Directory.Exists(strDestFolder))
            {
                Directory.CreateDirectory(strDestFolder);
            }

            var files = GetAllFiles(strSourceFolder, false);

            foreach (var file in files)
            {
                FileInfo fileInfo = new FileInfo(file);

                File.Copy(file, Path.Combine(strDestFolder, fileInfo.Name), true);
            }
        }

        internal static void DeleteFile(string strFilePath)
        {
            if (File.Exists(strFilePath))
            {
                File.Delete(strFilePath);
            }
        }

        internal static void DeleteFiles(string strFolder, List<string> lstIgnoreFileNames, bool isIncludeSubDirectories = false)
        {
            var files = GetAllFiles(strFolder, isIncludeSubDirectories);

            foreach (var file in files)
            {
                FileInfo fileInfo = new FileInfo(file);

                if (!lstIgnoreFileNames.Contains(fileInfo.Name))
                {
                    FileUtils.DeleteFile(file);
                }
            }
        }
    }
}