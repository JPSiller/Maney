using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Capuchinos.Maney.Dependencies;
using Capuchinos.Maney.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Capuchinos.Maney.Extensions
{
    [ContentProperty("Text")]
    internal class TranslateExtension : IMarkupExtension
    {
        private readonly CultureInfo _mCi;
        private const string ResourceId = "Capuchinos.Maney.Resources.ManeyResources";
        public string Text { get; set; }

        public TranslateExtension()
        {
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                Settings settings = DataRepository.GetSettings();
                if (!string.IsNullOrWhiteSpace(settings?.DefaultLanguage))
                {
                    Language language = DataRepository.GetLanguage(settings.DefaultLanguage);
                    if (language != null)
                    {
                        _mCi = new CultureInfo(language.Culture);
                        DependencyService.Get<ILocalization>().SetLocale(_mCi);
                    }
                }

                if (_mCi == null)
                {
                    _mCi = DependencyService.Get<ILocalization>().GetCurrentCultureInfo();
                }
            }
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            var temp = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);
            var translation = temp.GetString(Text, _mCi);
            return translation;
        }
    }
}
