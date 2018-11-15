using CoreGraphics;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class TriangleChatView : UIView 
    {
        UIColor backgroundColor;
        bool mirrored;

        public new UIColor BackgroundColor 
        {
            get => backgroundColor;
            set 
            {
                backgroundColor = value;
                SetNeedsDisplay();
            }
        }

        public bool Mirrored 
        {
            get => mirrored;
            set 
            {
                mirrored = value;
                SetNeedsDisplay();
            }
        }

        public TriangleChatView() 
        {
            base.BackgroundColor = UIColor.Clear;
        }

        public override void Draw(CGRect rect)
        {
            var context = UIGraphics.GetCurrentContext();
            context.BeginPath();

            if (mirrored)
            {
                context.MoveTo(rect.GetMaxX(), rect.GetMinY());
                context.AddLineToPoint(rect.GetMaxX(), rect.GetMaxY());
                context.AddLineToPoint(rect.GetMinX(), rect.GetMinY());
            }
            else
            {
                context.MoveTo(rect.GetMinX(), rect.GetMinY());
                context.AddLineToPoint(rect.GetMinX(), rect.GetMaxY());
                context.AddLineToPoint(rect.GetMaxX(), rect.GetMinY());
            }

            context.ClosePath();
            context.SetFillColor(backgroundColor?.CGColor ?? UIColor.Black.CGColor);
            context.FillPath();
        }
    }
}
