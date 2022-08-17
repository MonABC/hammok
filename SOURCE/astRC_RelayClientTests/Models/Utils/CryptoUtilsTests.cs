using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils.Tests
{
    [TestClass()]
    public class CryptoUtilsTests
    {
        [TestMethod()]
        public void DecryptFromFileTest()
        {
            const string filePath = "enc.txt";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            const string source = "abcd1234";
            Encrypt.EncryptToFile(filePath, source);

            Assert.IsTrue(File.Exists(filePath));
            Assert.IsTrue(1 < new FileInfo(filePath).Length);

            CollectionAssert.AreNotEqual(
                Encrypt.Encoding.GetBytes(source),
                File.ReadAllBytes(filePath));

            var result = Encrypt.DecryptFromFile(filePath);
            Assert.AreEqual(result, source);
        }
    }
}

