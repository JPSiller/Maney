using Capuchinos.Maney.CustomRenderers;
using Capuchinos.Maney.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomTabbedPage), typeof(CustomTabbedPageRenderer))]
namespace Capuchinos.Maney.iOS.CustomRenderers
{
    internal class CustomTabbedPageRenderer : TabbedRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                MoreNavigationController.Delegate = new CustomNavigationControllerDelegate();

                MoreNavigationController.NavigationBar.Translucent = false;
                MoreNavigationController.NavigationBar.BackgroundColor = UIColor.FromRGB(245, 222, 179);
                var vc = MoreNavigationController.ViewControllers[0];
                var tableView = (UITableView)vc.View;
                tableView.BackgroundColor = UIColor.FromRGB(255, 243, 231);
                tableView.TintColor = UIColor.FromRGB(197, 184, 179);
            }
        }
    }

    internal class CustomNavigationControllerDelegate : UINavigationControllerDelegate
    {
        public override void WillShowViewController(UINavigationController navigationController,
            UIViewController viewController, bool animated)
        {
            viewController.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(245, 222, 179);
            var tableView = viewController.View as UITableView;
            if (tableView != null)
            {
                tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                foreach (var cell in tableView.VisibleCells)
                {
                    cell.BackgroundColor = UIColor.FromRGB(255, 243, 231);
                    cell.Accessory = UITableViewCellAccessory.None;
                }
            }
        }
    }
}
