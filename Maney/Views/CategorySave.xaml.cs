using Xamarin.Forms.Xaml;

namespace Capuchinos.Maney.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategorySave
    {
        public CategorySave()
        {
            InitializeComponent();
            BindingContext = App.Locator.CategorySave;
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }
    }
}