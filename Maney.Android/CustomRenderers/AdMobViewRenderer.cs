using Capuchinos.Maney.CustomRenderers;
using Capuchinos.Maney.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobViewRenderer))]
namespace Capuchinos.Maney.Droid.CustomRenderers
{
    public class AdMobViewRenderer : ViewRenderer<AdMobView, Android.Gms.Ads.AdView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var ad = new Android.Gms.Ads.AdView(Forms.Context)
                {
                    AdSize = Android.Gms.Ads.AdSize.Banner,
                    AdUnitId = "ca-app-pub-7314872527484882/6908487859"
                };
                var requestbuilder = new Android.Gms.Ads.AdRequest.Builder();
                ad.LoadAd(requestbuilder.Build());
                SetNativeControl(ad);
            }
        }
    }
}