using Microsoft.Xaml.Behaviors;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Behaviors
{
    /// <summary>
    /// ユーザーが勝手にウィンドウを閉じることができないようにするため、閉じるボタンを非表示にするためのクラス
    /// </summary>
    public class HideCloseButtonOnWindowBehavior : Behavior<Window>
    {
        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

            [DllImport("user32.dll")]
            public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        }

        private const uint SC_CLOSE = 0xF060;

        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnLoaded;
            base.OnDetaching();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(AssociatedObject).Handle;
            IntPtr hMenu = NativeMethods.GetSystemMenu(hwnd, false);
            _ = NativeMethods.EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
        }
    }
}
