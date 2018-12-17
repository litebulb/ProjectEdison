using System;
using Edison.Mobile.Admin.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.Views
{
    public class CheckBoxItemView : UIView
    {
        readonly CheckBoxView checkboxView;
        readonly UILabel label;
        readonly UITapGestureRecognizer tapGestureRecognizer;

        bool selected;

        UIColor textColor;
        UIColor selectedTextColor;

        public event EventHandler OnTap;

        public string Text
        {
            get => label.Text;
            set => label.Text = value;
        }

        public UIColor TextColor
        {
            get => textColor;
            set
            {
                textColor = value;

                if (!selected)
                {
                    label.TextColor = textColor;
                }
            }
        }

        public UIColor SelectedTextColor
        {
            get => selectedTextColor;
            set
            {
                selectedTextColor = value;

                if (selected)
                {
                    label.TextColor = selectedTextColor;
                }
            }
        }

        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                label.TextColor = selected ? SelectedTextColor : TextColor;
                checkboxView.Selected = selected;
            }
        }

        public CheckBoxItemView()
        {
            checkboxView = new CheckBoxView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = Constants.Color.DarkBlue,
                UserInteractionEnabled = false,
            };

            AddSubview(checkboxView);

            checkboxView.LeftAnchor.ConstraintEqualTo(LeftAnchor, constant: Constants.Padding).Active = true;
            checkboxView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
            checkboxView.WidthAnchor.ConstraintEqualTo(24).Active = true;
            checkboxView.HeightAnchor.ConstraintEqualTo(checkboxView.WidthAnchor).Active = true;

            label = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = Constants.Color.DarkGray,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eighteen),
            };

            AddSubview(label);
            label.LeftAnchor.ConstraintEqualTo(checkboxView.RightAnchor, constant: Constants.Padding * 2).Active = true;
            label.CenterYAnchor.ConstraintEqualTo(checkboxView.CenterYAnchor).Active = true;
            label.RightAnchor.ConstraintEqualTo(RightAnchor, constant: -Constants.Padding).Active = true;

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
            Selected = !Selected;
            OnTap?.Invoke(this, new EventArgs());
        }
    }
}
