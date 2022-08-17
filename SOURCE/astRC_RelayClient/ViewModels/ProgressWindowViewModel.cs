using Prism.Mvvm;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.ViewModels
{
    public class ProgressWindowViewModel : BindableBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }
    }
}
