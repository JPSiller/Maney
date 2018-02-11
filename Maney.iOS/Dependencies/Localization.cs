using System;
using System.Globalization;
using System.Threading;
using Capuchinos.Maney.Dependencies;
using Capuchinos.Maney.Helpers;
using Capuchinos.Maney.iOS.Dependencies;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(Localization))]
namespace Capuchinos.Maney.iOS.Dependencies
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
            var netLanguage = "en";
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];

                netLanguage = OsToDotnetLanguage(pref);
            }

            CultureInfo ci;
            try
            {
                ci = new CultureInfo(netLanguage);
            }
            catch (CultureNotFoundException)
            {
                try
                {
                    var fallback = ToDotnetFallbackLanguage(new PlatformCulture(netLanguage));
                    ci = new CultureInfo(fallback);
                }
                catch (CultureNotFoundException)
                {
                    ci = new CultureInfo("en");
                }
            }

            return ci;
        }

        private static string OsToDotnetLanguage(string iOsLanguage)
        {
            var netLanguage = iOsLanguage;


            switch (iOsLanguage)
            {
                case "ms-MY":
                case "ms-SG":
                    netLanguage = "ms";
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
                case "pt":
                    netLanguage = "pt-PT";
                    break;
                case "gsw":
                    netLanguage = "de-CH";
                    break;
            }

            return netLanguage;
        }
    }
}
