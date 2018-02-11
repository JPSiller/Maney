using Foundation;
using Google.MobileAds;
using OxyPlot.Xamarin.Forms.Platform.iOS;
using UIKit;

namespace Capuchinos.Maney.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Forms.Forms.Init();
            PlotViewRenderer.Init();
            MobileAds.Configure("ca-app-pub-7314872527484882~2705287457");
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
