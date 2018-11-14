using System;
using Edison.Mobile.User.Client.iOS.Shared;
using UIKit;
namespace Edison.Mobile.User.Client.iOS.Views
{
    public class CircleAvatarView : UIView
    {
        UIImageView imageView;
        UILabel initialsLabel;

        public UIImage Image
        {
            get => imageView?.Image;
            set
            {
                if (imageView == null)
                {
                    imageView = new UIImageView { TranslatesAutoresizingMaskIntoConstraints = false };
                    AddSubview(imageView);
                    imageView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
                    imageView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
                    imageView.WidthAnchor.ConstraintEqualTo(WidthAnchor, multiplier: 0.5f).Active = true;
                    imageView.HeightAnchor.ConstraintEqualTo(HeightAnchor, multiplier: 0.5f).Active = true;
                }

                imageView.Image = value;
            }
        }

        public string Initials
        {
            get => initialsLabel?.Text;
            set
            {
                if (initialsLabel == null)
                {
                    initialsLabel = new UILabel
                    {
                        TranslatesAutoresizingMaskIntoConstraints = false,
                        AdjustsFontSizeToFitWidth = true,
                        MinimumScaleFactor = 0.1f,
                        LineBreakMode = UILineBreakMode.Clip,
                        Lines = 0,
                        TextColor = Constants.Color.White,
                        TextAlignment = UITextAlignment.Center,
                        Font = Constants.Fonts.RubikMediumOfSize(Constants.Fonts.Size.SeventyTwo),
                    };
                    AddSubview(initialsLabel);

                    initialsLabel.HeightAnchor.ConstraintEqualTo(HeightAnchor, multiplier: 0.75f).Active = true;
                    initialsLabel.WidthAnchor.ConstraintEqualTo(WidthAnchor, multiplier: 0.75f).Active = true;
                    initialsLabel.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
                    initialsLabel.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
                }

                initialsLabel.Text = value;
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (imageView != null) BringSubviewToFront(imageView);

            Layer.CornerRadius = Bounds.Height / 2;
        }
    }
}
