using System;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels
{
    public class ClientListViewItem
    {
        public string SessionId { get; set; }
        public string SessionState { get; set; }
        public DateTime ExpiredTime { get; set; }
        public string RcServerHostName { get; set; }
        public string RcServerUserName { get; set; }
        public string RcClientHostName { get; set; }
        public string RcClientUserName { get; set; }
    }
}
