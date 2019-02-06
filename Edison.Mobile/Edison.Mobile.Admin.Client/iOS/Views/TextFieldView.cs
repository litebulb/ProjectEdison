using System;
using CoreGraphics;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.Admin.Client.iOS.Extensions;
using UIKit;
using Foundation;

namespace Edison.Mobile.Admin.Client.iOS.Views
{
    public class TextFieldView : UIView, IUITextFieldDelegate
    {
        UILabel label;
        UITextField textField;
        UITapGestureRecognizer tapGestureRecognizer;

        public event EventHandler OnTextFieldViewReturned;
        public event EventHandler OnEditingBegan;
        public event EventHandler<UITextFieldDidEndEditingReason> OnEditingEnded;

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

        public bool SecureTextEntry
        {
            get => textField.SecureTextEntry;
            set => textField.SecureTextEntry = value;
        }

        public UIReturnKeyType ReturnKeyType
        {
            get => textField.ReturnKeyType;
            set => textField.ReturnKeyType = value;
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
                Delegate = this,
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

        [Export("textFieldShouldReturn:")]
        public bool ShouldReturn(UITextField textField)
        {
            OnTextFieldViewReturned?.Invoke(this, new EventArgs());
            return true;
        }

        [Export("textFieldShouldBeginEditing:")]
        public bool ShouldBeginEditing(UITextField textField)
        {
            OnEditingBegan?.Invoke(this, new EventArgs());
            return true;
        }

        [Export("textFieldDidEndEditing:reason:")]
        public void EditingEnded(UITextField textField, UITextFieldDidEndEditingReason reason)
        {
            OnEditingEnded?.Invoke(this, reason);
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

        public new void BecomeFirstResponder()
        {
            textField.BecomeFirstResponder();
        }

        public new bool ResignFirstResponder()
        {
            return textField.ResignFirstResponder();
        }

        void HandleTap()
        {
            textField.BecomeFirstResponder();
        }
    }
}
