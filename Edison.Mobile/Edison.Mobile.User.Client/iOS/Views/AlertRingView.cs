using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class AlertRingView : UIView
    {
        readonly nfloat startAngle = 1.5f * (nfloat)Math.PI;

        CAShapeLayer ringLayer;
        nfloat ringThickness = 20;
        UIColor ringColor = UIColor.White;

        public UIColor RingColor 
        {
            get => ringColor;
            set 
            {
                ringColor = value;
                SetNeedsLayout();
            }
        }

        public nfloat RingThickness 
        {
            get => ringThickness;
            set 
            {
                ringThickness = value;
                SetNeedsLayout();
            }
        }


        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (ringLayer == null) 
            {
                ringLayer = new CAShapeLayer
                {
                    FillColor = UIColor.Clear.CGColor,
                    StrokeColor = ringColor.CGColor,
                    LineWidth = ringThickness,
                    Frame = Bounds,
                };

                Layer.AddSublayer(ringLayer);
            }

            var radius = (Frame.Size.Width - ringThickness) / 2;
            var circlePath = UIBezierPath.FromArc(new CGPoint(Bounds.GetMidX(), Bounds.GetMidY()), radius, startAngle, startAngle + (2 * (nfloat)Math.PI), true);
            ringLayer.Path = circlePath.CGPath;
            ringLayer.StrokeEnd = 1;
        }
    }
}
