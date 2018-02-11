using GalaSoft.MvvmLight.Messaging;
using Xamarin.Forms.Xaml;

namespace Capuchinos.Maney.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = App.Locator.Main;
            Messenger.Default.Register<NotificationMessage<short>>(this, "NavigateToTab", NavigateToTab);
            Messenger.Default.Register<NotificationMessage>(this, "CleanUpView", CleanUpView);
        }

        private void NavigateToTab(NotificationMessage<short> notification)
        {
            CurrentPage = Children[notification.Content];
        }

        private void CleanUpView(NotificationMessage message)
        {
            Messenger.Default.Unregister<NotificationMessage>(this);
        }
    }
}