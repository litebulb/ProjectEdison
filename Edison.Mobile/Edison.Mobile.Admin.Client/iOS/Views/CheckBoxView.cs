using System;
using Edison.Mobile.Admin.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.Views
{
    public class CheckBoxView : UIView 
    {
        readonly UIImageView checkImageView;
        readonly UITapGestureRecognizer tapGestureRecognizer;

        bool selected;

        public event EventHandler OnTap;

        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                checkImageView.Hidden = !selected;
            }
        }

        public CheckBoxView(bool selected = false)
        {
            checkImageView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = Constants.Assets.Check,
            };

            AddSubview(checkImageView);

            checkImageView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
            checkImageView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
            checkImageView.WidthAnchor.ConstraintEqualTo(WidthAnchor, multiplier: 0.5f).Active = true;
            checkImageView.HeightAnchor.ConstraintEqualTo(HeightAnchor, multiplier: 0.5f).Active = true;

            tapGestureRecognizer = new UITapGestureRecognizer();
            tapGestureRecognizer.AddTarget(HandleTap);

            Selected = selected;
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
