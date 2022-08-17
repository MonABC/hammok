using System.IO;

namespace Hammock.AssetView.Platinum.Tools.Utils
{
    internal class FolderUtils
    {
        internal static void DeleteFolder(string strFolder)
        {
            if (Directory.Exists(strFolder))
            {
                Directory.Delete(strFolder, true);
            }
        }
    }
}