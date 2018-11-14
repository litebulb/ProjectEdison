using System;
using Edison.Mobile.User.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class MenuProfileTableViewCell : BaseMenuTableViewCell
    {
        readonly nfloat avatarHeight = (UIScreen.MainScreen.Bounds.Width - Constants.MenuRightMargin) / 2f;

        CircleAvatarView avatarView;

        public MenuProfileTableViewCell(IntPtr handle) : base(handle) { }

        public void Initialize(string name, UIImage profileImage = null, float fontSize = 12, string initials = null)
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
                    TextColor = Constants.Color.Black,
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

                avatarView = new CircleAvatarView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    BackgroundColor = Constants.Color.LightGray,
                };

                avatarView.Layer.CornerRadius = avatarHeight / 2;

                ContentView.AddSubview(avatarView);

                avatarView.BottomAnchor.ConstraintEqualTo(labelContainerView.TopAnchor).Active = true;
                avatarView.HeightAnchor.ConstraintEqualTo(avatarHeight).Active = true;
                avatarView.WidthAnchor.ConstraintEqualTo(avatarHeight).Active = true;
                avatarView.CenterXAnchor.ConstraintEqualTo(avatarLayoutGuide.CenterXAnchor).Active = true;

                isInitialized = true;
            }

            titleLabel.Text = name;
            avatarView.Image = profileImage;
            avatarView.Initials = initials;
        }
    }
}
