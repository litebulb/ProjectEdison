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
        UILabel subtitleLabel;

        public PulloutLargeButtonCollectionViewCell(IntPtr handle) : base(handle) { }

        public void Initialize(UIColor backgroundColor, UIImage assetImage, string title, string subtitle = null)
        {
            if (!isInitialized)
            {
                circleView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };

                AddSubview(circleView);

                circleView.LeftAnchor.ConstraintEqualTo(LeftAnchor, circleMargin).Active = true;
                circleView.RightAnchor.ConstraintEqualTo(RightAnchor, -circleMargin).Active = true;
                circleView.TopAnchor.ConstraintEqualTo(TopAnchor, circleMargin).Active = true;
                circleView.AddConstraint(NSLayoutConstraint.Create(circleView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, circleView, NSLayoutAttribute.Width, 1, 0));

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
                    TextColor = PlatformConstants.Color.DarkGray,
                };

                AddSubview(titleLabel);

                titleLabel.CenterXAnchor.ConstraintEqualTo(circleView.CenterXAnchor).Active = true;
                titleLabel.TopAnchor.ConstraintEqualTo(circleView.BottomAnchor, textVerticalMargin).Active = true;

                subtitleLabel = new UILabel
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Ten),
                    TextColor = PlatformConstants.Color.MidGray,
                };

                AddSubview(subtitleLabel);

                subtitleLabel.CenterXAnchor.ConstraintEqualTo(circleView.CenterXAnchor).Active = true;
                subtitleLabel.TopAnchor.ConstraintEqualTo(titleLabel.BottomAnchor, textVerticalMargin / 2).Active = true;

                isInitialized = true;
            }

            circleView.BackgroundColor = backgroundColor;
            assetImageView.Image = assetImage;
            titleLabel.Text = title;
            subtitleLabel.Text = subtitle;
            subtitleLabel.Alpha = string.IsNullOrEmpty(subtitle) ? 0 : 1;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            circleView.Layer.CornerRadius = circleView.Frame.Width / 2;
        }
    }
}
