using System;
using Xamarin.Forms;

namespace Capuchinos.Maney.Converters
{
    public class IsPurchasedToRowHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (System.Convert.ToBoolean(value))
            {
                return new GridLength(1, GridUnitType.Auto);
            }

            return new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }
}
