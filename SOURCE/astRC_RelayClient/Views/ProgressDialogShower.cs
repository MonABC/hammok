using Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels;
using Hammock.AssetView.Platinum.Tools.RC.RelayClient.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Views
{
    public class ProgressDialogShower : IProgressDialogShower
    {
        private readonly Window _owner;
        private readonly Dispatcher dispatcher;

        public ProgressDialogShower(Window owner)
        {
            _owner = owner;
            dispatcher = owner.Dispatcher;
        }

        public void ShowProgressWindowDialog(Action action, string title)
        {
            var viewModel = new ProgressWindowViewModel();
            {
                ProgressWindow window = null;

                dispatcher.Invoke(new Action(() =>
                {
                    viewModel.Title = title;

                    window = new ProgressWindow(viewModel);
                    window.Owner = _owner;
                }));

                var task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        action.Invoke();
                    }
                    finally
                    {
                        dispatcher.Invoke(new Action(() =>
                        {
                            window.Close();
                        }));
                    }
                });

                dispatcher.Invoke(new Action(() =>
                {
                    _ = window.ShowDialog();
                }));

                task.Wait();
            }
        }
    }
}
