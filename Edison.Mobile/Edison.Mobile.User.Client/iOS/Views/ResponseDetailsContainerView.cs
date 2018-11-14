using CoreGraphics;
using MapKit;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class ResponseDetailsContainerView : UIView
    {
        MKMapView mapView;
        UITableView tableView;
        ResponseUpdateTableViewHeaderView headerView;
        UIView iconBackgroundView;

        public override UIView HitTest(CGPoint point, UIEvent uievent)
        {
            var yOffset = tableView.ContentOffset.Y;
            var baseHitTestResult = base.HitTest(point, uievent);
            if (yOffset >= 0 || baseHitTestResult == iconBackgroundView)
            {
                return baseHitTestResult;
            }

            var mapY = -yOffset + tableView.Frame.Top - headerView.Frame.Height;

            if (point.Y <= mapY)
            {
                return mapView.HitTest(mapView.ConvertPointFromView(point, this), uievent);
            }

            return base.HitTest(point, uievent);
        }

        public override void LayoutSubviews()
        {
            if (mapView == null || tableView == null)
            {
                foreach (var subview in Subviews)
                {
                    if (subview is MKMapView mView) mapView = mView;
                    if (subview is UITableView tView) tableView = tView;
                    if (subview is ResponseUpdateTableViewHeaderView hView) headerView = hView;
                    if (subview.Tag == 1) iconBackgroundView = subview;
                }
            }

            base.LayoutSubviews();
        }
    }
}
