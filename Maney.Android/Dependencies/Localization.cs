using System;
using System.Globalization;
using System.Threading;
using Capuchinos.Maney.Dependencies;
using Capuchinos.Maney.Droid.Dependencies;
using Capuchinos.Maney.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(Localization))]
namespace Capuchinos.Maney.Droid.Dependencies
{
    public class Localization : ILocalization
    {
        public void SetLocale(CultureInfo ci)
        {
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            Console.WriteLine(@"CurrentCulture set: " + ci.Name);
        }

        public CultureInfo GetCurrentCultureInfo()
        {
            var androidLocale = Java.Util.Locale.Default;
            var netLanguage = AndroidToDotnetLanguage(androidLocale.ToString().Replace("_", "-"));

            CultureInfo ci;
            try
            {
                ci = new CultureInfo(netLanguage);
            }
            catch (CultureNotFoundException e1)
            {
                try
                {
                    var fallback = ToDotnetFallbackLanguage(new PlatformCulture(netLanguage));
                    Console.WriteLine(netLanguage + @" failed, trying " + fallback + @" (" + e1.Message + @")");
                    ci = new CultureInfo(fallback);
                }
                catch (CultureNotFoundException e2)
                {
                    Console.WriteLine(netLanguage + @" couldn't be set, using 'en' (" + e2.Message + @")");
                    ci = new CultureInfo("en");
                }
            }

            return ci;
        }

        private static string AndroidToDotnetLanguage(string androidLanguage)
        {
            var netLanguage = androidLanguage;

            switch (androidLanguage)
            {
                case "ms-BN":
                case "ms-MY":
                case "ms-SG":
                    netLanguage = "ms";
                    break;
                case "in-ID":
                    netLanguage = "id-ID";
                    break;
                case "gsw-CH":
                    netLanguage = "de-CH";
                    break;
            }

            return netLanguage;
        }

        private static string ToDotnetFallbackLanguage(PlatformCulture platCulture)
        {
            var netLanguage = platCulture.LanguageCode;

            switch (platCulture.LanguageCode)
            {
                case "gsw":
                    netLanguage = "de-CH";
                    break;
            }

            return netLanguage;
        }
    }
}