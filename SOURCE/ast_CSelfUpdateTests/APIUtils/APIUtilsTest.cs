using Hammock.AssetView.Platinum.Tools.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hammock.AssetView.Platinum.Tools
{
    [TestClass]
    public class APIUtilsTest
    {
        [TestMethod]
        public void GetNewestVersionTest()
        {
            string softwareName = "ast_CAutoUpdate";
            string actual = APIUtils.GetNewestVersion(softwareName).Result;
            string expected = "1.1.1.1";
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void GetNewestVersionEmptyTest()
        {
            string softwareName = "softwareNameEmpty";
            string actual = APIUtils.GetNewestVersion(softwareName).Result;
            Assert.AreEqual(actual, string.Empty);
        }

        [TestMethod]
        public void NullExceptionTest()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await APIUtils.GetNewestVersion(null));
        }

        [TestMethod]
        public void AuthentExceptionTest()
        {
            Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () => await APIUtils.GetNewestVersion("ast_CAutoUpdate"));
        }

        [TestMethod]
        public void CheckDownloadNullTest()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await APIUtils.DownloadNewestInstallerFile(null, "1.1.1.1"));

            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await APIUtils.DownloadNewestInstallerFile("ast_CAutoUpdate", null));
        }
    }
}
