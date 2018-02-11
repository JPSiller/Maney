using Xamarin.Forms;

namespace Capuchinos.Maney.CustomRenderers
{
    public class ExtendedEntry : Entry
    {
        public static readonly BindableProperty IsValidProperty =
            BindableProperty.Create("IsValid", typeof(bool), typeof(ExtendedEntry), true);

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }
    }
}
