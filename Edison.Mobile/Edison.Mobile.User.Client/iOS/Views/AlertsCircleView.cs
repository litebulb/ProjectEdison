using System;
using CoreAnimation;
using CoreGraphics;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.User.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class AlertsCircleView : UIView
    {
        readonly UILabel eventsLabel;
        readonly UILabel alertCountLabel;
        readonly UIView innerCircleView;
        readonly UIImageView locationImageView;
        readonly AlertRingView ringView;
        readonly nfloat innerCircleMargin = 15;

        int alertCount;
        NSLayoutConstraint locationImageViewRightAnchorConstraint;
        NSLayoutConstraint locationImageViewCenterYAnchorConstraint;
        NSLayoutConstraint alertCountLabelCenterYAnchorConstraint;

        public UIColor Color
        {
            get => innerCircleView.BackgroundColor;
            set
            {
                InvokeOnMainThread(() =>
                {
                    innerCircleView.BackgroundColor = value;
                    ringView.RingColor = value;
                    SetNeedsDisplay();
                });
            }
        }

        public int AlertCount 
        {
            get => alertCount;
            set 
            {
                alertCount = value;
                alertCountLabel.Text = alertCount.ToString();
                eventsLabel.Text = alertCount == 1 ? "ALERT" : "ALERTS";
            }
        }

        public AlertsCircleView()
        {
            innerCircleView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = UIColor.White };
            AddSubview(innerCircleView);
            innerCircleView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true; 
            innerCircleView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
            innerCircleView.WidthAnchor.ConstraintEqualTo(WidthAnchor, constant: -(2 * innerCircleMargin)).Active = true;
            innerCircleView.HeightAnchor.ConstraintEqualTo(innerCircleView.WidthAnchor).Active = true;

            alertCountLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "0",
                TextColor = Constants.Color.White,
                TextAlignment = UITextAlignment.Center,
            };

            innerCircleView.AddSubview(alertCountLabel);
            alertCountLabel.CenterXAnchor.ConstraintEqualTo(innerCircleView.CenterXAnchor).Active = true;
            alertCountLabelCenterYAnchorConstraint = alertCountLabel.CenterYAnchor.ConstraintEqualTo(innerCircleView.CenterYAnchor, -(innerCircleView.Bounds.Height / 12));
            alertCountLabelCenterYAnchorConstraint.Active = true;

            eventsLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.White,
                Text = "ALERTS",
                TextAlignment = UITextAlignment.Center,
            };

            locationImageView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = Constants.Assets.Location,
            };

            var eventsLabelContainerView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            eventsLabelContainerView.AddSubview(eventsLabel);
            eventsLabelContainerView.AddSubview(locationImageView);
            locationImageViewRightAnchorConstraint = locationImageView.RightAnchor.ConstraintEqualTo(eventsLabel.LeftAnchor);
            locationImageViewRightAnchorConstraint.Active = true;
            locationImageViewCenterYAnchorConstraint = locationImageView.CenterYAnchor.ConstraintEqualTo(eventsLabel.CenterYAnchor);
            locationImageViewCenterYAnchorConstraint.Active = true;
            eventsLabel.RightAnchor.ConstraintEqualTo(eventsLabelContainerView.RightAnchor).Active = true;
            eventsLabel.BottomAnchor.ConstraintEqualTo(eventsLabelContainerView.BottomAnchor).Active = true;
            locationImageView.LeftAnchor.ConstraintEqualTo(eventsLabelContainerView.LeftAnchor).Active = true;
            locationImageView.TopAnchor.ConstraintEqualTo(eventsLabelContainerView.TopAnchor).Active = true;

            innerCircleView.AddSubview(eventsLabelContainerView);
            eventsLabelContainerView.TopAnchor.ConstraintEqualTo(alertCountLabel.BottomAnchor).Active = true;
            eventsLabelContainerView.CenterXAnchor.ConstraintEqualTo(innerCircleView.CenterXAnchor).Active = true;

            ringView = new AlertRingView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                RingColor = Color,
                RingThickness = innerCircleMargin / 2,
            };

            AddSubview(ringView);

            ringView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            ringView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            ringView.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            ringView.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            innerCircleView.Layer.CornerRadius = innerCircleView.Bounds.Height / 2;
            ringView.RingThickness = innerCircleMargin / 2;

            var alertCountFontSize = GetRelativeFloat(72);
            var eventsFontSize = GetRelativeFloat(14);
            alertCountLabel.Font = Constants.Fonts.RubikOfSize(alertCountFontSize);
            eventsLabel.Font = Constants.Fonts.RubikOfSize(eventsFontSize);
            SetNeedsUpdateConstraints();
        }

        public override void UpdateConstraints()
        {
            base.UpdateConstraints();

            alertCountLabelCenterYAnchorConstraint.Constant = -(innerCircleView.Bounds.Height / GetRelativeFloat(12));
            locationImageViewRightAnchorConstraint.Constant = -GetRelativeFloat(6);
            locationImageViewCenterYAnchorConstraint.Constant = -GetRelativeFloat(4);
        }

        public void StartAnimating() 
        {
            ringView.StartAnimating();
        }

        public void StopAnimating() 
        {
            ringView.StopAnimating();
        }

        float GetRelativeFloat(float originalSize) => (float) (Bounds.Height > 0 ? Bounds.Height : 200) * (originalSize / 200f);
    }
}
