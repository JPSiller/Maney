using System;
using System.Globalization;
using Capuchinos.Maney.Dependencies;
using Capuchinos.Maney.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Capuchinos.Maney.Extensions
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        private const string EnPath = "Capuchinos.Maney.Images.en.";
        private const string EsPath = "Capuchinos.Maney.Images.es.";
        private const string FrPath = "Capuchinos.Maney.Images.fr.";
        private const string JaPath = "Capuchinos.Maney.Images.ja.";
        private const string PtPath = "Capuchinos.Maney.Images.pt.";
        private const string ZhPath = "Capuchinos.Maney.Images.zh.";
        public string Source { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
                return null;

            Settings settings = DataRepository.GetSettings();
            CultureInfo mCi = null;
            if (!string.IsNullOrWhiteSpace(settings?.DefaultLanguage))
            {
                Language language = DataRepository.GetLanguage(settings.DefaultLanguage);
                if (language != null)
                {
                    mCi = new CultureInfo(language.Culture);
                    DependencyService.Get<ILocalization>().SetLocale(mCi);
                }
            }

            if (mCi == null)
            {
                mCi = DependencyService.Get<ILocalization>().GetCurrentCultureInfo();
            }

            string imagePath;
            if (Source.IndexOf('.') != Source.LastIndexOf('.'))
            {
                imagePath = Source;
            }
            else
            {
                if (mCi.Name.Contains("en"))
                {
                    imagePath = string.Concat(EnPath, Source);
                }
                else if (mCi.Name.Contains("es"))
                {
                    imagePath = string.Concat(EsPath, Source);
                }
                else if (mCi.Name.Contains("ja"))
                {
                    imagePath = string.Concat(JaPath, Source);
                }
                else if (mCi.Name.Contains("pt"))
                {
                    imagePath = string.Concat(PtPath, Source);
                }
                else if (mCi.Name.Contains("fr"))
                {
                    imagePath = string.Concat(FrPath, Source);
                }
                else if (mCi.Name.Contains("zh"))
                {
                    imagePath = string.Concat(ZhPath, Source);
                }
                else
                {
                    imagePath = string.Concat(EnPath, Source);
                }
            }

            var imageSource = ImageSource.FromResource(imagePath);

            return imageSource;
        }
    }
}
