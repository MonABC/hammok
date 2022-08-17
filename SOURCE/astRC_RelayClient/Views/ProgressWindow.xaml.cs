using Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels;
using System.Windows;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Views
{
    /// <summary>
    /// ProgressWindow.xaml の相互作用ロジック
    /// </summary>
    internal partial class ProgressWindow : Window
    {
        public ProgressWindow(ProgressWindowViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MinHeight = RenderSize.Height;
            MaxHeight = RenderSize.Height;
        }
    }
}
