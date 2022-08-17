using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hammock.AssetView.Platinum.Tools.Utils;
using System.IO;

namespace Hammock.AssetView.Platinum.Tools
{
    [TestClass]
    public class AutoUpdateUtilsTest
    {
        [TestMethod]
        public void GetNewestVersionTest()
        {
            string tempFolder = Path.Combine(AutoUpdateSettings.PARENT_FOLDER, "UnitTest");
            string strCompletedFilePath = Path.Combine(tempFolder, AutoUpdateSettings.COMPLETED_FILE_NAME);
            string strVersion = "1.1.1.1";

            if (File.Exists(strCompletedFilePath))
            {
                File.Delete(strCompletedFilePath);
            }
            
            AutoUpdateUtils.CreateCompletedFile(tempFolder, strVersion);
            Assert.IsTrue(File.Exists(strCompletedFilePath));
        }
    }
}
