using System;
using Xamarin.Forms;

namespace Capuchinos.Maney.Converters
{
    public class TransactionTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((short)value == 0)
                return Color.FromHex("#30B55C");

            return Color.FromHex("#F55255");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
