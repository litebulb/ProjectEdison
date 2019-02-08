using System;
using Edison.Mobile.Admin.Client.iOS.Extensions;
using Edison.Mobile.Admin.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.Views
{
    public class SwitchFieldView : UIView
    {
        UILabel label;
        UISwitch switchView;
        UITapGestureRecognizer tapGestureRecognizer;

        public event EventHandler<bool> OnSwitchValueChanged;

        public string LabelText
        {
            get => label.Text;
            set => label.Text = value;
        }

        public bool On
        {
            get => switchView.On;
            set => switchView.On = value;
        }

        public SwitchFieldView()
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

            switchView = new UISwitch
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            AddSubview(switchView);

            switchView.RightAnchor.ConstraintEqualTo(RightAnchor, constant: -Constants.Padding).Active = true;
            switchView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;

            HeightAnchor.ConstraintEqualTo(48).Active = true;

            BackgroundColor = Constants.Color.White;

            tapGestureRecognizer = new UITapGestureRecognizer();
            tapGestureRecognizer.AddTarget(HandleTap);

            this.AddStandardShadow();
        }

        void SwitchView_EditingChanged(object sender, EventArgs e)
        {
            Console.WriteLine(e);
        }

        void HandleTap()
        {
            //switchView.On = !switchView.On;
            switchView.SetState(!switchView.On, true);
            OnSwitchValueChanged?.Invoke(this, switchView.On);
        }

        public override void WillMoveToWindow(UIWindow window)
        {
            if (window == null)
            {
                RemoveGestureRecognizer(tapGestureRecognizer);
                switchView.EditingChanged += SwitchView_EditingChanged;
            }
            else
            {
                AddGestureRecognizer(tapGestureRecognizer);
                switchView.EditingChanged += SwitchView_EditingChanged;
            }
        }
    }
}
