using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Common;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Views
{
    /// <summary>
    /// ServerWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ServerWindow : UserControl
    {
        public ServerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (ServerWindowViewModel)DataContext;

            var win = Window.GetWindow(this);
            win.MinHeight = win.RenderSize.Height;
            win.MaxHeight = win.RenderSize.Height;
            viewModel.ProgressDialogShower = new ProgressDialogShower(win);
            viewModel.MessageDialog = new WpfMessageDialog(Dispatcher, win);
            viewModel.Dispatcher = Dispatcher;

            var owner = win.Owner;
            win.Owner = null;
            owner.Close();
        }
    }
}
