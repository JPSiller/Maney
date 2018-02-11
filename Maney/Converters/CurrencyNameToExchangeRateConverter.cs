using System;
using Capuchinos.Maney.Model;
using Capuchinos.Maney.Resources;
using Xamarin.Forms;

namespace Capuchinos.Maney.Converters
{
    public class CurrencyNameToExchangeRateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            Transaction t = (Transaction)value;
            return $"(1 {t.BaseCurrency} = {t.ExchangeRate} {t.ToCurrency} {ManeyResources.By} {t.CurrencyRateDate})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
