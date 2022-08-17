using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Behaviors
{
    public class ListBoxItemClickBehavior : Behavior<ListBox>
    {
        public ICommand MouseLeftButtonDownCommand
        {
            get { return (ICommand)GetValue(MouseLeftButtonDownCommandProperty); }
            set { SetValue(MouseLeftButtonDownCommandProperty, value); }
        }

        public ICommand MouseDoubleClickCommand
        {
            get { return (ICommand)GetValue(MouseDoubleClickCommandProperty); }
            set { SetValue(MouseDoubleClickCommandProperty, value); }
        }

        public static readonly DependencyProperty MouseLeftButtonDownCommandProperty =
            DependencyProperty.Register("MouseLeftButtonDownCommand", typeof(ICommand), typeof(ListBoxItemClickBehavior), new PropertyMetadata(null));

        public static readonly DependencyProperty MouseDoubleClickCommandProperty =
            DependencyProperty.Register("MouseDoubleClickCommand", typeof(ICommand), typeof(ListBoxItemClickBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseDoubleClick += AssociatedObject_PreviewMouseDoubleClick;
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseDoubleClick -= AssociatedObject_PreviewMouseDoubleClick;
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;

            base.OnDetaching();
        }

        private void AssociatedObject_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ClickEvent(e.OriginalSource as DependencyObject, MouseDoubleClickCommand);
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClickEvent(e.OriginalSource as DependencyObject, MouseLeftButtonDownCommand);
        }

        private void ClickEvent(DependencyObject originalSource, ICommand command)
        {
            var listBoxItem = GetAncestor<ListBoxItem>(originalSource);
            if (listBoxItem == null) return;

            if (command != null)
            {
                var dataContext = listBoxItem.DataContext;

                if (command.CanExecute(dataContext))
                {
                    command.Execute(dataContext);
                }
            }
        }

        public static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject
        {
            DependencyObject parent = reference;

            for (; ; )
            {
                parent = VisualTreeHelper.GetParent(parent);
                if (parent == null || parent is T) break;
            }

            return (T)parent;
        }
    }
}
