using System;
using Capuchinos.Maney.Model;
using Capuchinos.Maney.ViewModel;
using Capuchinos.Maney.Views;
using GalaSoft.MvvmLight.Messaging;

namespace Capuchinos.Maney
{
    public partial class App
    {
        public static ViewModelLocator Locator => new ViewModelLocator();
        public App()
        {
            Messenger.Default.Register<NotificationMessage>(this, "ReloadApp", ReloadApp);
            DataRepository.Init();
            InitializeComponent();
            if (DataRepository.GetSettings() == null)
                MainPage = new InitialSettings();
            else
                MainPage = new MainPage();
        }

        private void ReloadApp(NotificationMessage message)
        {
            MainPage = new MainPage();
        }
    }
}
