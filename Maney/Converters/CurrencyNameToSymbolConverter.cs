using System;
using Capuchinos.Maney.Model;
using Xamarin.Forms;

namespace Capuchinos.Maney.Converters
{
    public class CurrencyNameToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            return DataRepository.GetCurrencySymbol(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
