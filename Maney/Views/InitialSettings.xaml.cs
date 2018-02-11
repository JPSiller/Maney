using GalaSoft.MvvmLight.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Capuchinos.Maney.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InitialSettings
    {
        public InitialSettings()
        {
            InitializeComponent();
            BindingContext = App.Locator.InitialSettings;
            Messenger.Default.Register<NotificationMessage>(this, "Start", Start);
        }

        private void Start(NotificationMessage message)
        {
            Application.Current.MainPage = new MainPage();
        }
    }
}