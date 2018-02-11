using Capuchinos.Maney.CustomRenderers;
using Capuchinos.Maney.iOS.CustomRenderers;
using CoreGraphics;
using Google.MobileAds;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobViewRenderer))]
namespace Capuchinos.Maney.iOS.CustomRenderers
{
    public class AdMobViewRenderer : ViewRenderer
    {
        private const string AdmobId = "ca-app-pub-7314872527484882/4182020659";
        BannerView _adView;
        bool _viewOnScreen;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            if (e.OldElement == null)
            {
                UIViewController viewCtrl = null;
                foreach (UIWindow v in UIApplication.SharedApplication.Windows)
                {
                    if (v.RootViewController != null)
                    {
                        viewCtrl = v.RootViewController;
                    }
                }

                _adView = new BannerView(AdSizeCons.Banner, new CGPoint(-10, 0))
                {
                    AdUnitID = AdmobId,
                    RootViewController = viewCtrl
                };

                _adView.AdReceived += (sender, args) =>
                {
                    if (!_viewOnScreen) AddSubview(_adView);
                    _viewOnScreen = true;
                };

                var request = Request.GetDefaultRequest();
                request.TestDevices = new[] {Request.SimulatorId.ToString()};

                _adView.LoadRequest(request);
                SetNativeControl(_adView);
            }
        }
    }
}
