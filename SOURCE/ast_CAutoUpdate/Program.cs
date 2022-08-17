using System.ServiceProcess;

namespace Hammock.AssetView.Platinum.Tools
{
    internal static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AutoUpdateService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}