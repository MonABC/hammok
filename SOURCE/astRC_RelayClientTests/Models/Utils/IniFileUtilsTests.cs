using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils.Tests
{
    [TestClass()]
    public class IniFileUtilsTests
    {
        [TestMethod()]
        public void SetValueTest()
        {
            const string iniFilePath = @"TestDatas\Models\Utils\Sample01.ini";
            const string section = "ClientMode";
            IniFileUtils.SetValue(section, "boot", "1", iniFilePath);

            CollectionAssert.AreEqual(
                File.ReadAllBytes(iniFilePath),
                File.ReadAllBytes(@"TestDatas\Models\Utils\Sample01-expected01.ini"));

            IniFileUtils.SetValue(section, null, null, iniFilePath);

            CollectionAssert.AreEqual(
                File.ReadAllBytes(iniFilePath),
                File.ReadAllBytes(@"TestDatas\Models\Utils\Sample01-expected02.ini"));
        }

        [TestMethod()]
        public void GetValueTest()
        {
            string iniFilePath = Path.GetFullPath(@"TestDatas\Models\Utils\Sample02.ini");
            Assert.IsTrue(File.Exists(iniFilePath));

            const string section = "Section1";
            var value = IniFileUtils.GetValue(section, "key1", "", iniFilePath);
            Assert.AreEqual("ABCDEFG", value);
        }
    }
}


