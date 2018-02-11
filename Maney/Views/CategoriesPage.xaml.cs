using GalaSoft.MvvmLight.Messaging;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace Capuchinos.Maney.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategoriesPage
    {
        public CategoriesPage()
        {
            InitializeComponent();
            BindingContext = App.Locator.Category;
            Messenger.Default.Register<NotificationMessage>(this, "CategorySave", PromptCategorySave);
            Messenger.Default.Register<NotificationMessage>(this, "CleanUpView", CleanUpView);
        }

        private async void PromptCategorySave(NotificationMessage message)
        {
            await PopupNavigation.PushAsync(new CategorySave());
        }

        private void CleanUpView(NotificationMessage message)
        {
            Messenger.Default.Unregister<NotificationMessage>(this);
        }
    }
}