using Hammock.AssetView.Platinum.Tools.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hammock.AssetView.Platinum.Tools.Controllers
{
    [TestClass()]
    public class DownloadLatestVersionControllerTests
    {
        [TestMethod()]
        public void DownloadLatestVersionTest()
        {
            // arrange
            var model  = new DownloadLatestVersionModel
            {
                SoftwareName = "test",
                Version = "1.2.3.4",
            };

            // action
            var stream = new DownloadLatestVersionController().DownloadLatestVersion(
                  new AuthenticateModel() { UserName = AutoUpdateServiceSettings.API_USERNAME, Password = AutoUpdateServiceSettings.API_PASSWORD}
                , model
                );

            var actualBytes = new byte[1024];
            var readedLength = stream.Read(actualBytes, 0, actualBytes.Length);

            // assert
            var expectedBytes = File.ReadAllBytes(@"TestDatas\DownloadLatestVersion\installer.zip");
            CollectionAssert.AreEqual(expectedBytes, actualBytes.Take(readedLength).ToArray());
        }
    }
}

