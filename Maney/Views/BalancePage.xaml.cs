using Xamarin.Forms.Xaml;

namespace Capuchinos.Maney.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BalancePage
    {
        public BalancePage()
        {
            InitializeComponent();
            BindingContext = App.Locator.Balance;
        }
    }
}