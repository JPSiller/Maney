using System.Globalization;

namespace Capuchinos.Maney.Dependencies
{
    public interface ILocalization
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);
    }
}
