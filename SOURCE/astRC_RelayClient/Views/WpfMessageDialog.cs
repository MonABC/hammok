using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Properties;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Views
{
    public class WpfMessageDialog : IMessageDialog
    {
        private static readonly string Title = Resources.ProgramName;
        private readonly Dispatcher dispatcher;
        private readonly Window ownerWindow;

        public WpfMessageDialog(Dispatcher disp, Window owner)
        {
            dispatcher = disp ?? throw new ArgumentNullException(nameof(disp));

            // appクラスから使用する場合、owner=nullとなるため、nullチェックは行わない。
            ownerWindow = owner;
        }

        public void Error(string errorMessage)
        {
            var button = MessageBoxButton.OK;
            var image = MessageBoxImage.Error;
            _ = ShowMessage(errorMessage, button, image);
        }

        public void Warn(string warnMessage)
        {
            var button = MessageBoxButton.OK;
            var image = MessageBoxImage.Warning;
            _ = ShowMessage(warnMessage, button, image);
        }

        public bool Confirm(string message)
        {
            var result = ShowMessage(message, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        private MessageBoxResult ShowMessage(string message, MessageBoxButton button, MessageBoxImage image)
        {
            return dispatcher.Invoke(() =>
            {
                if (ownerWindow == null)
                {
                    return MessageBox.Show(message, Title, button, image);
                }
                else
                {
                    return MessageBox.Show(ownerWindow, message, Title, button, image);
                }
            });
        }

        public void Info(string message)
        {
            var button = MessageBoxButton.OK;
            var image = MessageBoxImage.Information;
            _ = ShowMessage(message, button, image);
        }
    }
}