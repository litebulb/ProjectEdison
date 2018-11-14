using System;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.User.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class MenuItemTableViewCell : BaseMenuTableViewCell
    {
        readonly float logoBackgroundPadding = 8;

        UIView backgroundLogoCircleView;
        UIImageView logoImageView;

        public MenuItemTableViewCell(IntPtr handle) : base(handle) { }

        public void Initialize(string title, UIImage image = null)
        {
            if (!isInitialized)
            {
                backgroundLogoCircleView = new UIView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    BackgroundColor = Constants.Color.DarkBlue,
                };

                ContentView.AddSubview(backgroundLogoCircleView);

                backgroundLogoCircleView.LeftAnchor.ConstraintEqualTo(ContentView.LeftAnchor, constant: Constants.MenuRightMargin + Constants.Padding).Active = true;
                backgroundLogoCircleView.CenterYAnchor.ConstraintEqualTo(ContentView.CenterYAnchor).Active = true;
                backgroundLogoCircleView.TopAnchor.ConstraintEqualTo(TopAnchor, constant: logoBackgroundPadding).Active = true;
                backgroundLogoCircleView.BottomAnchor.ConstraintEqualTo(BottomAnchor, constant: -logoBackgroundPadding).Active = true;
                backgroundLogoCircleView.WidthAnchor.ConstraintEqualTo(backgroundLogoCircleView.HeightAnchor).Active = true;

                logoImageView = new UIImageView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Image = image,
                };

                ContentView.AddSubview(logoImageView);
                logoImageView.CenterXAnchor.ConstraintEqualTo(backgroundLogoCircleView.CenterXAnchor).Active = true;
                logoImageView.CenterYAnchor.ConstraintEqualTo(backgroundLogoCircleView.CenterYAnchor).Active = true;
                logoImageView.WidthAnchor.ConstraintEqualTo(backgroundLogoCircleView.WidthAnchor, constant: -20).Active = true;
                logoImageView.HeightAnchor.ConstraintEqualTo(backgroundLogoCircleView.HeightAnchor, constant: -20).Active = true;

                titleLabel = new UILabel
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    TextColor = Constants.Color.DarkBlue,
                    Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Sixteen),
                };

                ContentView.AddSubview(titleLabel);

                titleLabel.LeftAnchor.ConstraintEqualTo(backgroundLogoCircleView.RightAnchor, constant: Constants.Padding).Active = true;
                titleLabel.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor).Active = true;
                titleLabel.CenterYAnchor.ConstraintEqualTo(ContentView.CenterYAnchor).Active = true;

                isInitialized = true;
            }

            titleLabel.Text = title;

            SetNeedsLayout();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            backgroundLogoCircleView.Layer.CornerRadius = (Constants.MenuCellHeight - (logoBackgroundPadding * 2)) / 2f;
        }
    }
}
