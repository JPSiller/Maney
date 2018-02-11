using Capuchinos.Maney.Model;
using GalaSoft.MvvmLight.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Capuchinos.Maney.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomePage
    {
        private bool _isEditMode;
        public IncomePage()
        {
            InitializeComponent();
            BindingContext = App.Locator.Income;
            Messenger.Default.Register<NotificationMessage<Transaction>>(this, "EditIncomePageBehind", UpdateQuantity);
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