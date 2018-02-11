using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Services;

namespace Capuchinos.Maney.ViewModel
{
    public class HelpViewModel : ViewModelBase
    {
        public RelayCommand CloseCommand { get; }

        public HelpViewModel()
        {
            CloseCommand = new RelayCommand(Close);
        }

        private async void Close()
        {
            await PopupNavigation.PopAllAsync();
        }
    }
}
