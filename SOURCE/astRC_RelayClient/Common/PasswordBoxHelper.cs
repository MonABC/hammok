using System.Windows;
using System.Windows.Controls;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Common
{
    public class PasswordBoxHelper : DependencyObject
    {
        public static readonly DependencyProperty IsAttachedProperty = DependencyProperty.RegisterAttached(
            "IsAttached",
            typeof(bool),
            typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(false, PasswordBoxHelper.IsAttachedProperty_Changed));

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached(
            "Password",
            typeof(string),
            typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PasswordBoxHelper.PasswordProperty_Changed));

        public static bool GetIsAttached(DependencyObject dp)
        {
            if (dp == null)
            {
                throw new System.ArgumentNullException(nameof(dp));
            }
            return (bool)dp.GetValue(PasswordBoxHelper.IsAttachedProperty);
        }

        public static string GetPassword(DependencyObject dp)
        {
            if (dp == null)
            {
                throw new System.ArgumentNullException(nameof(dp));
            }
            return (string)dp.GetValue(PasswordBoxHelper.PasswordProperty);
        }

        public static void SetIsAttached(DependencyObject dp, bool value)
        {
            if (dp == null)
            {
                throw new System.ArgumentNullException(nameof(dp));
            }
            dp.SetValue(PasswordBoxHelper.IsAttachedProperty, value);
        }

        public static void SetPassword(DependencyObject dp, string value)
        {
            if (dp == null)
            {
                throw new System.ArgumentNullException(nameof(dp));
            }
            dp.SetValue(PasswordBoxHelper.PasswordProperty, value);
        }

        private static void IsAttachedProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordBoxHelper.PasswordBox_PasswordChanged;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordBoxHelper.PasswordBox_PasswordChanged;
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            PasswordBoxHelper.SetPassword(passwordBox, passwordBox.Password);
        }

        private static void PasswordProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var newPassword = (string)e.NewValue;

            if (!GetIsAttached(passwordBox))
            {
                SetIsAttached(passwordBox, true);
            }

            if ((string.IsNullOrEmpty(passwordBox.Password) && string.IsNullOrEmpty(newPassword)) ||
                passwordBox.Password == newPassword)
            {
                return;
            }

            passwordBox.PasswordChanged -= PasswordBoxHelper.PasswordBox_PasswordChanged;
            passwordBox.Password = newPassword;
            passwordBox.PasswordChanged += PasswordBoxHelper.PasswordBox_PasswordChanged;
        }
    }
}
