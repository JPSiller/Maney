using Xamarin.Forms.Xaml;

namespace Capuchinos.Maney.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage
    {
        public HistoryPage()
        {
            InitializeComponent();
            BindingContext = App.Locator.History;
        }
    }
}