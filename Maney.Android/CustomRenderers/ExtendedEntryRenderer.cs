using System.ComponentModel;
using Android.Graphics;
using Capuchinos.Maney.CustomRenderers;
using Capuchinos.Maney.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedEntry), typeof(ExtendedEntryRenderer))]
namespace Capuchinos.Maney.Droid.CustomRenderers
{
    internal class ExtendedEntryRenderer : EntryRenderer
    {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName.Equals(nameof(ExtendedEntry.IsValid)))
            {
                var control = (ExtendedEntry)sender;
                if (!control.IsValid)
                {
                    Control.Background.SetColorFilter(Android.Graphics.Color.Red, PorterDuff.Mode.SrcAtop);
                }
                else
                {
                    Control.Background.ClearColorFilter();
                }
            }
        }
    }
}