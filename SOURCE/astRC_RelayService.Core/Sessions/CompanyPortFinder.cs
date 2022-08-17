using Hammock.AssetView.Platinum.Tools.RC.RelayService.Common;
using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Sessions
{
    public class CompanyPortFinder
    {
        private readonly Random random = new Random();
        private readonly Dictionary<string, List<ushort>> _ports = new Dictionary<string, List<ushort>>();

        public CompanyPortFinder()
        {
            var settingDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Hammock\PLATINUM\Server");
            if (Directory.Exists(settingDirectoryPath) == false)
            {
                // デバッグ中にこの例外が発生した場合、「デバッグ用ファイル」に記載したファイルを所定の場所にコピーして使用すること。
                throw new DirectoryNotFoundException(settingDirectoryPath);
            }
            var portsFilePath = Path.Combine(settingDirectoryPath, "astRC_RelayService_CompanyPorts.esf");
            if (File.Exists(portsFilePath) == false)
            {
                throw new FileNotFoundException(portsFilePath);
            }
            var content = Encrypt.DecryptFromFile(portsFilePath);

            CompanyPorts companyPorts;
            using (var reader = new StringReader(content))
            {
                var seri = new XmlSerializer(typeof(CompanyPorts));
                companyPorts = (CompanyPorts)seri.Deserialize(reader);
            }

            foreach (var companyPort in companyPorts)
            {
                _ports.Add(companyPort.CompanyID, companyPort.AvailablePorts);
            }
        }

        internal ushort Find(string companyId)
        {
            var availablePorts = _ports[companyId];
            var index = this.random.Next(availablePorts.Count);
            return availablePorts[index];
        }
    }
}