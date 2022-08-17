using System;
using CommandLine;

namespace Hammock.AssetView.Platinum.Tools
{
    internal class Args
    {
        [Option('v', "version", Required = true, HelpText = "Version")]
        public string Version { get; set; }

        [Option('c', "check", Required = true, HelpText = "Check")]
        public string Check { get; set; }
    }
}
