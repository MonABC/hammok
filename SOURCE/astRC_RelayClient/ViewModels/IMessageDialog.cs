namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels
{
    public interface IMessageDialog
    {
        void Error(string message);
        void Warn(string message);
        bool Confirm(string message);
        void Info(string message);
    }
}

