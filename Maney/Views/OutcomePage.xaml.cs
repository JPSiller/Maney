using Capuchinos.Maney.Model;
using GalaSoft.MvvmLight.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Capuchinos.Maney.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OutcomePage
    {
        private bool _isEditMode;
        public OutcomePage()
        {
            InitializeComponent();
            BindingContext = App.Locator.Outcome;
            Messenger.Default.Register<NotificationMessage<Transaction>>(this, "EditOutcomePageBehind", UpdateQuantity);
            Messenger.Default.Register<NotificationMessage>(this, "CleanUpView", CleanUpView);
        }

        private void UpdateQuantity(NotificationMessage<Transaction> result)
        {
            OneWayQuantity.Text = result.Content.Quantity.ToString();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    _isEditMode = true;
                    break;
            }
        }

        protected override void OnAppearing()
        {
            if (!_isEditMode)
                OneWayQuantity.Text = string.Empty;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    _isEditMode = false;
                    break;
            }
            base.OnAppearing();
        }

        private void CleanUpView(NotificationMessage message)
        {
            Messenger.Default.Unregister<NotificationMessage>(this);
        }
    }
}