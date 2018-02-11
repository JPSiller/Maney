using System.ComponentModel;
using Capuchinos.Maney.CustomRenderers;
using Capuchinos.Maney.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedEntry), typeof(ExtendedEntryRenderer))]
namespace Capuchinos.Maney.iOS.CustomRenderers
{
    internal class ExtendedEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;

            Control.Layer.BorderColor = UIColor.LightGray.CGColor;
            Control.Layer.BorderWidth = 0.25f;
            Control.Layer.CornerRadius = 5;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName.Equals(nameof(ExtendedEntry.IsValid)))
            {
                var control = (ExtendedEntry)sender;
                if (!control.IsValid)
                {
                    Control.Layer.BorderColor = UIColor.Red.CGColor;
                    Control.Layer.BorderWidth = 1;
                    Control.Layer.CornerRadius = 5;
                }
                else
                {
                    Control.Layer.BorderColor = UIColor.LightGray.CGColor;
                    Control.Layer.BorderWidth = 0.25f;
                    Control.Layer.CornerRadius = 5;
                }
            }
        }
    }
}
