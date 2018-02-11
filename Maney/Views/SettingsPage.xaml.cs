using GalaSoft.MvvmLight.Messaging;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace Capuchinos.Maney.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = App.Locator.Settings;
            Messenger.Default.Register<NotificationMessage>(this, "Help", PromptHelp);
            Messenger.Default.Register<NotificationMessage>(this, "CleanUpView", CleanUpView);
        }

        private async void PromptHelp(NotificationMessage message)
        {
            await PopupNavigation.PushAsync(new Help());
        }

        private void CleanUpView(NotificationMessage message)
        {
            Messenger.Default.Unregister<NotificationMessage>(this);
        }
    }
}