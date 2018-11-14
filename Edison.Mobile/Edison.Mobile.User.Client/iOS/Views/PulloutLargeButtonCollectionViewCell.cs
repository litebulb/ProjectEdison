using System;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.iOS.Common.Views;
using Edison.Mobile.User.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class PulloutLargeButtonCollectionViewCell : BaseCollectionViewCell
    {
        readonly nfloat circleMargin = 18;
        readonly nfloat textVerticalMargin = 8;
        readonly float assetScaleFactor = 0.5f;
        UIView circleView;
        UIImageView assetImageView;
        UILabel titleLabel;

        NSLayoutConstraint widthConstraint;

        public PulloutLargeButtonCollectionViewCell(IntPtr handle) : base(handle) { }

        public void Initialize(UIColor backgroundColor, UIImage assetImage, string title, string subtitle = null)
        {
            if (!isInitialized)
            {
                circleView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };

                AddSubview(circleView);

                circleView.CenterXAnchor.ConstraintEqualTo(ContentView.CenterXAnchor).Active = true;
                circleView.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
                widthConstraint = circleView.WidthAnchor.ConstraintEqualTo(ContentView.WidthAnchor, constant: -(2 * circleMargin));
                widthConstraint.Active = true;
                circleView.HeightAnchor.ConstraintEqualTo(circleView.WidthAnchor).Active = true;

                assetImageView = new UIImageView { TranslatesAutoresizingMaskIntoConstraints = false };

                AddSubview(assetImageView);

                assetImageView.CenterYAnchor.ConstraintEqualTo(circleView.CenterYAnchor).Active = true;
                assetImageView.CenterXAnchor.ConstraintEqualTo(circleView.CenterXAnchor).Active = true;
                assetImageView.WidthAnchor.ConstraintEqualTo(circleView.WidthAnchor, assetScaleFactor).Active = true;
                assetImageView.HeightAnchor.ConstraintEqualTo(circleView.HeightAnchor, assetScaleFactor).Active = true;

                titleLabel = new UILabel
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Fourteen),
                    TextColor = Constants.Color.DarkGray,
                };

                AddSubview(titleLabel);

                titleLabel.CenterXAnchor.ConstraintEqualTo(circleView.CenterXAnchor).Active = true;
                titleLabel.TopAnchor.ConstraintEqualTo(circleView.BottomAnchor, textVerticalMargin).Active = true;

                isInitialized = true;
            }

            circleView.BackgroundColor = backgroundColor;
            assetImageView.Image = assetImage;
            titleLabel.Text = title;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            circleView.Layer.CornerRadius = circleView.Frame.Width / 2;
        }

        public void SetPercentMinimized(nfloat percentMinimized) 
        {
            var maximizedWidthConstant = -(2 * circleMargin);
            var minimizedWidthConstant = ContentView.Frame.Width / 2;
            widthConstraint.Constant = maximizedWidthConstant - (minimizedWidthConstant * percentMinimized);

            titleLabel.Alpha = 1 - (percentMinimized * 2);

            LayoutIfNeeded();
        }
    }
}
