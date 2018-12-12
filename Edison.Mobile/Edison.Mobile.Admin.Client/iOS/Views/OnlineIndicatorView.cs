using System;
using CoreGraphics;
using Edison.Mobile.Admin.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.Views
{
    public class OnlineIndicatorView : UIView
    {
        bool isOnline;

        public bool IsOnline
        {
            get => isOnline;
            set
            {
                isOnline = value;
                SetNeedsDisplay();
            }
        }

        public override void Draw(CGRect rect)
        {
            var context = UIGraphics.GetCurrentContext();

            (isOnline ? Constants.Color.White : Constants.Color.DarkGray).SetStroke();

            var center = new CGPoint(rect.GetMidX(), rect.GetMidY());
            var radius = 6f;

            context.SetLineWidth(1);
            var path = UIBezierPath.FromArc(center, radius, 0, (nfloat)Math.PI * 2, true);

            if (isOnline)
            {
                Constants.Color.Green.SetFill();
                path.Fill();
            }
            else
            {
                path.Stroke();
            }
        }
    }
}
