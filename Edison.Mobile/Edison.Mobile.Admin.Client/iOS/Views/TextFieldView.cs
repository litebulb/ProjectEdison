using System;
using CoreGraphics;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.Admin.Client.iOS.Extensions;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.Views
{
    public class TextFieldView : UIView
    {
        UILabel label;
        UITextField textField;
        UITapGestureRecognizer tapGestureRecognizer;

        public string LabelText
        {
            get => label.Text;
            set => label.Text = value;
        }

        public string Text
        {
            get => textField.Text;
            set => textField.Text = value;
        }

        public TextFieldView()
        {
            var textViewLeftMarginPercent = 0.3f;

            label = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.DarkGray,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Fourteen),
            };

            AddSubview(label);

            label.LeftAnchor.ConstraintEqualTo(LeftAnchor, constant: Constants.Padding).Active = true;
            label.WidthAnchor.ConstraintEqualTo(WidthAnchor, multiplier: textViewLeftMarginPercent, constant: -Constants.Padding).Active = true;
            label.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;

            textField = new UITextField
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Fourteen),
                TextColor = Constants.Color.MidGray,
            };

            AddSubview(textField);

            textField.LeftAnchor.ConstraintEqualTo(label.RightAnchor, constant: Constants.Padding).Active = true;
            textField.RightAnchor.ConstraintEqualTo(RightAnchor, constant: -Constants.Padding).Active = true;
            textField.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            textField.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;

            HeightAnchor.ConstraintEqualTo(48).Active = true;

            BackgroundColor = Constants.Color.White;

            tapGestureRecognizer = new UITapGestureRecognizer();
            tapGestureRecognizer.AddTarget(HandleTap);

            this.AddStandardShadow();
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
            textField.BecomeFirstResponder();
        }
    }
}
