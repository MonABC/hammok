using System.Windows;
using System.Windows.Controls;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Views
{
    /// <summary>
    /// SwitchWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SwitchWindow : UserControl
    {
        public SwitchWindow()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            win.MinHeight = win.RenderSize.Height;
            win.MaxHeight = win.RenderSize.Height;
        }
    }
}
