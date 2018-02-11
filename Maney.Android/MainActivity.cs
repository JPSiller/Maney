using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using OxyPlot.Xamarin.Forms.Platform.Android;
using Plugin.InAppBilling;
using Xamarin.Forms.Platform.Android;
using Forms = Xamarin.Forms.Forms;

namespace Capuchinos.Maney.Droid
{
    [Activity(
        Label = "Maney",
        Icon = "@drawable/icon",
        Theme = "@style/MainTheme",
        MainLauncher = false,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            UserDialogs.Init(() => (Activity)Forms.Context);
            PlotViewRenderer.Init();
            MobileAds.Initialize(ApplicationContext, "ca-app-pub-7314872527484882~4323451454");
            LoadApplication(new App());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }
    }
}

