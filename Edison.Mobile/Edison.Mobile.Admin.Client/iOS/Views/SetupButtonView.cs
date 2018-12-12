using System;
using CoreGraphics;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Foundation;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.Views
{
    public class SetupButtonView : UIView
    {
        readonly nfloat padding = 16;
        readonly UITapGestureRecognizer tapGestureRecognizer;

        UILabel label;
        UIImageView largeIconImageView;
        UIImageView smallIconImageView;

        public event EventHandler OnTap;

        public UIImage LargeIconImage
        {
            get => largeIconImageView.Image;
            set => largeIconImageView.Image = value;
        }

        public UIImage SmallIconImage
        {
            get => smallIconImageView.Image;
            set => smallIconImageView.Image = value;
        }

        public string Text
        {
            get => label.Text;
            set => label.Text = value;
        }

        public SetupButtonView()
        {
            Layer.ShadowColor = Constants.Color.DarkGray.CGColor;
            Layer.ShadowRadius = 3;
            Layer.ShadowOffset = new CGSize(1, 1);
            Layer.ShadowOpacity = 0.4f;
            Layer.MasksToBounds = false;

            smallIconImageView = new UIImageView { TranslatesAutoresizingMaskIntoConstraints = false };
            AddSubview(smallIconImageView);
            smallIconImageView.RightAnchor.ConstraintEqualTo(RightAnchor, constant: -padding / 2).Active = true;
            smallIconImageView.TopAnchor.ConstraintEqualTo(TopAnchor, constant: padding).Active = true;
            smallIconImageView.WidthAnchor.ConstraintEqualTo(20).Active = true;
            smallIconImageView.HeightAnchor.ConstraintEqualTo(smallIconImageView.WidthAnchor).Active = true;

            largeIconImageView = new UIImageView { TranslatesAutoresizingMaskIntoConstraints = false };
            AddSubview(largeIconImageView);
            largeIconImageView.RightAnchor.ConstraintEqualTo(smallIconImageView.LeftAnchor, constant: -padding / 2).Active = true;
            largeIconImageView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
            largeIconImageView.HeightAnchor.ConstraintEqualTo(30).Active = true;
            largeIconImageView.WidthAnchor.ConstraintEqualTo(largeIconImageView.HeightAnchor).Active = true;

            label = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
                TextColor = Constants.Color.DarkBlue,
            };
            AddSubview(label);
            label.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
            label.LeftAnchor.ConstraintEqualTo(LeftAnchor, constant: padding).Active = true;
            label.RightAnchor.ConstraintEqualTo(largeIconImageView.LeftAnchor, constant: padding / 2).Active = true;

            tapGestureRecognizer = new UITapGestureRecognizer();
            tapGestureRecognizer.AddTarget(HandleTap);
        }

        public override void WillMoveToWindow(UIWindow window)
        {
            if (window == null)
            {
                RemoveGestureRecognizer(tapGestureRecognizer);
            }
            else
            {
                AddGestureRecognizer(tapGestureRecognizer);
            }
        }

        void HandleTap()
        {
            OnTap?.Invoke(this, new EventArgs());
        }
    }
}
