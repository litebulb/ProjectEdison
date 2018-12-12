using System;
using UIKit;
using CoreGraphics;
using Edison.Mobile.Admin.Client.iOS.Shared;

namespace Edison.Mobile.Admin.Client.iOS.Views
{
    public class CircleNumberView : UIView
    {
        readonly UILabel label;

        public UIColor CircleBackgroundColor { get; set; } = Constants.Color.MidGray;

        public int Number
        {
            get => int.Parse(label.Text);
            set => label.Text = value.ToString();
        }

        public UIColor TextColor
        {
            get => label.TextColor;
            set => label.TextColor = value;
        }

        public CircleNumberView()
        {
            label = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Center,
                AdjustsFontSizeToFitWidth = true,
                MinimumScaleFactor = 0.1f,
                LineBreakMode = UILineBreakMode.Clip,
                Lines = 0,
            };

            AddSubview(label);

            label.HeightAnchor.ConstraintEqualTo(HeightAnchor).Active = true;
            label.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
            label.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
            label.WidthAnchor.ConstraintEqualTo(WidthAnchor).Active = true;

            TextColor = Constants.Color.White;
            BackgroundColor = UIColor.Clear;
        }

        public override void Draw(CGRect rect)
        {
            var context = UIGraphics.GetCurrentContext();

            var center = new CGPoint(rect.GetMidX(), rect.GetMidY());
            var radius = (nfloat)Math.Min(Bounds.Width / 2, Bounds.Height / 2);

            context.SetLineWidth(1);
            var path = UIBezierPath.FromArc(center, radius, 0, (nfloat)Math.PI * 2, true);

            CircleBackgroundColor.SetFill();
            path.Fill();
        }
    }
}
