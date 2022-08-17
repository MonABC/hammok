using Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Views
{
    /// <summary>
    /// ClientWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ClientWindow : UserControl
    {
        public ClientWindow()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            var viewModel = (ClientWindowViewModel)DataContext;
            viewModel.ProgressDialogShower = new ProgressDialogShower(win);
            viewModel.MessageDialog = new WpfMessageDialog(Dispatcher, win);

            var owner = win.Owner;
            win.Owner = null;
            owner.Close();
        }
    }
}
