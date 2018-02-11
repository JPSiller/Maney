using System;
using Capuchinos.Maney.Model;
using Xamarin.Forms;

namespace Capuchinos.Maney.Converters
{
    public class StringToDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null && System.Convert.ToBoolean(parameter))
            {
                Settings settings = DataRepository.GetSettings();
                string symbol = DataRepository.GetCurrencySymbol(settings.DefaultCurrency);

                return $"{symbol}{(decimal)value:N4}";
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace((string)value))
                return 0d;

            double result;

            if (double.TryParse(value.ToString(), out result))
                return System.Convert.ToDecimal(value);

            return result;
        }
    }
}
