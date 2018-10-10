using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.User.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class PulloutMaximizedView : UIView
    {
        readonly UILabel sendMessageLabel;

        public PulloutMaximizedView()
        {
            sendMessageLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.TwentyFour),
                TextColor = PlatformConstants.Color.DarkGray,
                Text = "Send Message",
            };

            AddSubview(sendMessageLabel);

            sendMessageLabel.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            sendMessageLabel.LeftAnchor.ConstraintEqualTo(LeftAnchor, Constants.Padding).Active = true;
            sendMessageLabel.RightAnchor.ConstraintEqualTo(RightAnchor, -Constants.Padding).Active = true;
        }
    }
}
