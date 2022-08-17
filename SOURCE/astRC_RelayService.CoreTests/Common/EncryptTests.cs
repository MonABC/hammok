using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common.Tests
{
    [TestClass()]
    public class EncryptTests
    {
        [TestMethod()]
        public void EncryptToStringTest01()
        {
            const string source1 = "abcd1234";
            var encrypted = Encrypt.EncryptToString(source1);
            Assert.AreNotEqual(source1, encrypted);
            var source2 = Encrypt.DecryptToString(encrypted);
            Assert.AreEqual(source1, source2);
        }

        [TestMethod()]
        public void EncryptToStringTest02()
        {
            const string filePath = "test01.dat";
            const string source1 = "abcd1234efgh5678";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            Encrypt.EncryptToFile(filePath, source1);
            string encrypted1 = Convert.ToBase64String(File.ReadAllBytes(filePath));
            string encrypted2 = Encrypt.EncryptToString(source1);
            Assert.AreEqual(encrypted1, encrypted2);
        }

        [TestMethod()]
        public void EncryptToStringTest03()
        {
            const string filePath = @"TestDatas\test-file.dat";
            const string source1 = "abcd1234efgh5678";

            var source2 = Encrypt.DecryptFromFile(filePath);
            Assert.AreEqual(source1, source2);
        }
    }
}


