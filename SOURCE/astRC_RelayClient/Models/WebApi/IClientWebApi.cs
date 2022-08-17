using Hammock.AssetView.Platinum.Tools.RC.RelayService.Models.V1.Client;
using System.Collections.Generic;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.WebApi
{
    public interface IClientWebApi
    {
        string CloudUrl { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        IEnumerable<GetSessionResultModel> GetSessionList();
    }
}