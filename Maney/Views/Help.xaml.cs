using Xamarin.Forms.Xaml;

namespace Capuchinos.Maney.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Help
    {
        public Help()
        {
            InitializeComponent();
            BindingContext = App.Locator.Help;
        }
    }
}