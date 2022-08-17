using System;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels
{
    public interface IProgressDialogShower
    {
        void ShowProgressWindowDialog(Action action, string title);
    }
}