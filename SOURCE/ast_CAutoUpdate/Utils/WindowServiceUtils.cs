using System.ServiceProcess;

namespace Hammock.AssetView.Platinum.Tools.Utils
{
    internal class WindowServiceUtils
    {
        internal static void StopService(string strServiceName)
        {
            ServiceController service = new ServiceController(strServiceName);
            if (service.CanStop)
            {
                service.Stop();
            }
        }

        internal static void StartService(string strServiceName)
        {
            ServiceController service = new ServiceController(strServiceName);
            service.Start();
        }
    }
}
