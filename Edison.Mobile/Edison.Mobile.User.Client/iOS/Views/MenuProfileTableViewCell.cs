using System;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.User.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class MenuProfileTableViewCell : BaseMenuTableViewCell
    {
        readonly nfloat avatarHeight = (UIScreen.MainScreen.Bounds.Width - Constants.MenuRightMargin) / 2f;

        UIImageView avatarImageView;

        public MenuProfileTableViewCell(IntPtr handle) : base(handle) { }

        public void Initialize(string name, UIImage profileImage = null, float fontSize = 12)
        {
            if (!isInitialized)
            {
                var labelContainerView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };
                ContentView.AddSubview(labelContainerView);

                labelContainerView.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor).Active = true;
                labelContainerView.LeftAnchor.ConstraintEqualTo(ContentView.LeftAnchor, Constants.MenuRightMargin).Active = true;
                labelContainerView.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor).Active = true;
                labelContainerView.HeightAnchor.ConstraintEqualTo(Constants.MenuCellHeight).Active = true;

                titleLabel = new UILabel
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    TextColor = PlatformConstants.Color.White,
                    Font = Constants.Fonts.RubikOfSize(fontSize),
                    TextAlignment = UITextAlignment.Center,
                };

                labelContainerView.AddSubview(titleLabel);

                titleLabel.LeftAnchor.ConstraintEqualTo(labelContainerView.LeftAnchor).Active = true;
                titleLabel.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor).Active = true;
                titleLabel.CenterYAnchor.ConstraintEqualTo(labelContainerView.CenterYAnchor).Active = true;

                var avatarLayoutGuide = new UILayoutGuide();

                ContentView.AddLayoutGuide(avatarLayoutGuide);

                avatarLayoutGuide.LeftAnchor.ConstraintEqualTo(ContentView.LeftAnchor, Constants.MenuRightMargin).Active = true;
                avatarLayoutGuide.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor).Active = true;

                avatarImageView = new UIImageView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    BackgroundColor = PlatformConstants.Color.LightGray,
                };

                avatarImageView.Layer.CornerRadius = avatarHeight / 2;

                ContentView.AddSubview(avatarImageView);

                avatarImageView.BottomAnchor.ConstraintEqualTo(labelContainerView.TopAnchor).Active = true;
                avatarImageView.HeightAnchor.ConstraintEqualTo(avatarHeight).Active = true;
                avatarImageView.WidthAnchor.ConstraintEqualTo(avatarHeight).Active = true;
                avatarImageView.CenterXAnchor.ConstraintEqualTo(avatarLayoutGuide.CenterXAnchor).Active = true;

                isInitialized = true;
            }

            titleLabel.Text = name;
            avatarImageView.Image = profileImage;
        }
    }
}
