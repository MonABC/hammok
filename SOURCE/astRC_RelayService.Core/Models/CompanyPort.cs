using Newtonsoft.Json;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Models
{
    public class CompanyPort
    {
        public string CompanyID { get; set; }

        public AvailablePorts AvailablePorts { get; set; } = new AvailablePorts();
    }
}
