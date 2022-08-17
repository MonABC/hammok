using Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels;
using System.Windows;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Views
{
    /// <summary>
    /// LoginWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            LoginWindowViewModel viewModel = (LoginWindowViewModel)DataContext;
            viewModel.MessageDialog = new WpfMessageDialog(Dispatcher, this);
            viewModel.CloseEvent = () =>
            {
                Dispatcher.Invoke(() =>
                {
                    foreach (Window ownedWindow in OwnedWindows)
                    {
                        ownedWindow.Owner = null;
                    }
                    Close();
                });
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MinHeight = RenderSize.Height;
            MaxHeight = RenderSize.Height;
        }
    }
}
