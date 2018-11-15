using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    // https://stackoverflow.com/a/36655516
    public class RingLayer : CALayer
    {
        public RingLayer(CGRect bounds, CGPoint position, UIColor fromColor, UIColor toColor, nfloat lineWidth, nfloat toValue)
        {
            Bounds = bounds;
            Position = position;

            var colors = GetColors(fromColor, toColor, 4);

            var valuePositions = new List<CGPoint>
            {
                new CGPoint { X = (Bounds.Width / 4) * 3, Y = (Bounds.Height / 4) * 1 },
                new CGPoint { X = (Bounds.Width / 4) * 3, Y = (Bounds.Height / 4) * 3 },
                new CGPoint { X = (Bounds.Width / 4) * 1, Y = (Bounds.Height / 4) * 3 },
                new CGPoint { X = (Bounds.Width / 4) * 1, Y = (Bounds.Height / 4) * 1 },
            };

            var startPoints = new CGPoint[]
            {
                CGPoint.Empty,
                new CGPoint(1, 0),
                new CGPoint(1, 1),
                new CGPoint(0, 1),
            };

            var endPoints = new CGPoint[]
            {
                new CGPoint(1, 1),
                new CGPoint(0, 1),
                CGPoint.Empty,
                new CGPoint(1, 0),
            };

            for (var i = 0; i < colors.Count - 1; i++)
            {
                var gradientLayer = new CAGradientLayer
                {
                    Bounds = new CGRect
                    {
                        Location = CGPoint.Empty,
                        Size = new CGSize
                        {
                            Width = Bounds.Width / 2,
                            Height = Bounds.Height / 2,
                        },
                    },
                };

                var valuePosition = valuePositions[i];
                gradientLayer.Position = valuePosition;
                var fromCGColor = colors[i].CGColor;
                var toCGColor = colors[i + 1].CGColor;
                var gradientColors = new CGColor[] { fromCGColor, toCGColor };
                var locations = new NSNumber[] { 0, 1 };
                gradientLayer.Colors = gradientColors;
                gradientLayer.Locations = locations;
                gradientLayer.StartPoint = startPoints[i];
                gradientLayer.EndPoint = endPoints[i];
                AddSublayer(gradientLayer);

                var shapeLayer = new CAShapeLayer();
                var rect = new CGRect
                {
                    Location = CGPoint.Empty,
                    Size = new CGSize
                    {
                        Width = Bounds.Width - 2 * lineWidth,
                        Height = Bounds.Height - 2 * lineWidth,
                    },
                };

                shapeLayer.Bounds = rect;
                shapeLayer.Position = new CGPoint
                {
                    X = Bounds.Width / 2,
                    Y = Bounds.Height / 2,
                };

                shapeLayer.StrokeColor = UIColor.Blue.CGColor;
                shapeLayer.FillColor = UIColor.Clear.CGColor;
                shapeLayer.Path = UIBezierPath.FromRoundedRect(rect, rect.Width / 2).CGPath;
                shapeLayer.LineWidth = lineWidth;
                shapeLayer.LineCap = CAShapeLayer.CapSquare;
                shapeLayer.StrokeStart = 0.010f;

                var finalValue = toValue * 0.99f;

                shapeLayer.StrokeEnd = finalValue;

                Mask = shapeLayer;
            }
        }

        List<UIColor> GetColors(UIColor fromColor, UIColor toColor, nfloat gradientCount)
        {
            fromColor.GetRGBA(out nfloat fromR, out nfloat fromG, out nfloat fromB, out nfloat fromA);
            toColor.GetRGBA(out nfloat toR, out nfloat toG, out nfloat toB, out nfloat toA);

            var resultColors = new List<UIColor>();

            for (var i = 0; i <= gradientCount; i++)
            {
                var r = fromR + (toR - fromR) / gradientCount * i;
                var g = fromG + (toG - fromG) / gradientCount * i;
                var b = fromB + (toB - fromB) / gradientCount * i;
                var a = fromA + (toA - fromA) / gradientCount * i;
                var color = UIColor.FromRGBA(r, g, b, a);
                resultColors.Add(color);
            }

            return resultColors;
        }
    }

    public class AlertRingView : UIView
    {
        readonly nfloat startAngle = 1.5f * (nfloat)Math.PI;

        RingLayer ringLayer;
        UIView ringView;
        nfloat ringThickness = 20;
        UIColor ringColor = UIColor.White;
        bool isAnimating;

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

        void AnimateRing()
        {
            AnimateNotify(2f, 0, UIViewAnimationOptions.CurveLinear, () =>
            {
                ringView.Transform = CGAffineTransform.Rotate(ringView.Transform, (nfloat)Math.PI);
            }, finished =>
            {
                if (isAnimating && finished)
                {
                    AnimateRing();
                }
            });
        }

       public void StartAnimating()
        {
            isAnimating = true;
            AnimateRing();
        }

        public void StopAnimating()
        {
            isAnimating = false;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (ringView == null)
            {
                ringView = new UIView(Bounds);
                AddSubview(ringView);
                ringLayer = new RingLayer(Bounds, Center, ringColor, ringColor.ColorWithAlpha(0.5f), ringThickness, 1.1f);
                ringView.Layer.AddSublayer(ringLayer);
            }
        }
    }
}
