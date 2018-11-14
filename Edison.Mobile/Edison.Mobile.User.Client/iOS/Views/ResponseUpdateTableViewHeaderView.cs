using System;
using Edison.Mobile.User.Client.iOS.Shared;
using UIKit;
namespace Edison.Mobile.User.Client.iOS.Views
{
    public class ResponseUpdateTableViewHeaderView : UIView
    {
        readonly UILabel titleLabel;

        public ResponseUpdateTableViewHeaderView()
        {
            BackgroundColor = Constants.Color.BackgroundDarkGray;

            titleLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "ALERT UPDATES",
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.TwentyFour),
                TextColor = Constants.Color.White,
                BackgroundColor = UIColor.Clear,
            };

            AddSubview(titleLabel);

            titleLabel.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
            titleLabel.LeftAnchor.ConstraintEqualTo(LeftAnchor, constant: Constants.Padding).Active = true;
            titleLabel.RightAnchor.ConstraintEqualTo(RightAnchor, constant: -Constants.Padding).Active = true;

        }
    }
}
