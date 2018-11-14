using System;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.User.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class ChatMessageTypeCollectionViewCell : UICollectionViewCell
    {
        nfloat backgroundCircleSize; // TODO: make this dynamic based on collection view height
        readonly nfloat cellPadding = 4;
        readonly nfloat iconSizeFactor = 0.5f;

        bool isInitialized;

        UIView iconBackgroundView;
        UIImageView iconImageView;
        UILabel titleLabel;
        UIColor selectionColor;
        UIImage iconImage;
        UIImage selectedIconImage;

        public ChatMessageTypeCollectionViewCell(IntPtr handle) : base(handle)
        {
        }

        public void Initialize(nfloat cellHeight, ChatMessageType suggestion)
        {
            if (!isInitialized)
            {
                iconBackgroundView = new UIView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    BackgroundColor = Constants.Color.LightGray,
                };

                backgroundCircleSize = cellHeight * (38f / 60f);

                ContentView.TranslatesAutoresizingMaskIntoConstraints = false;
                ContentView.HeightAnchor.ConstraintEqualTo(cellHeight).Active = true;
                ContentView.WidthAnchor.ConstraintGreaterThanOrEqualTo(backgroundCircleSize + (2 * cellPadding)).Active = true;

                ContentView.AddSubview(iconBackgroundView);
                iconBackgroundView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, constant: cellPadding).Active = true;
                iconBackgroundView.CenterXAnchor.ConstraintEqualTo(ContentView.CenterXAnchor).Active = true;
                iconBackgroundView.WidthAnchor.ConstraintEqualTo(backgroundCircleSize).Active = true;
                iconBackgroundView.HeightAnchor.ConstraintEqualTo(backgroundCircleSize).Active = true;
                iconBackgroundView.Layer.CornerRadius = backgroundCircleSize / 2;

                iconImageView = new UIImageView { TranslatesAutoresizingMaskIntoConstraints = false };

                ContentView.AddSubview(iconImageView);
                iconImageView.CenterXAnchor.ConstraintEqualTo(iconBackgroundView.CenterXAnchor).Active = true;
                iconImageView.CenterYAnchor.ConstraintEqualTo(iconBackgroundView.CenterYAnchor).Active = true;
                iconImageView.WidthAnchor.ConstraintEqualTo(iconBackgroundView.WidthAnchor, multiplier: iconSizeFactor).Active = true;
                iconImageView.HeightAnchor.ConstraintEqualTo(iconBackgroundView.HeightAnchor, multiplier: iconSizeFactor).Active = true;

                titleLabel = new UILabel
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Ten),
                    TextColor = Constants.Color.MidGray,
                    TextAlignment = UITextAlignment.Center,
                };

                var titleLayoutGuide = new UILayoutGuide();
                ContentView.AddLayoutGuide(titleLayoutGuide);
                titleLayoutGuide.TopAnchor.ConstraintEqualTo(iconBackgroundView.BottomAnchor).Active = true;
                titleLayoutGuide.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor).Active = true;

                ContentView.AddSubview(titleLabel);
                titleLabel.LeftAnchor.ConstraintEqualTo(ContentView.LeftAnchor, constant: cellPadding).Active = true;
                titleLabel.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor, constant: -cellPadding).Active = true;
                titleLabel.CenterYAnchor.ConstraintEqualTo(titleLayoutGuide.CenterYAnchor).Active = true;

                isInitialized = true;
            }

            selectionColor = suggestion.SelectionColor;
            iconImage = suggestion.IconImage;
            selectedIconImage = suggestion.SelectedIconImage;
            titleLabel.Text = suggestion.Title;
            iconImageView.Image = iconImage;
        }

        public void SetSelected(bool selected)
        {
            Animate(0.05, () =>
            {
                iconBackgroundView.BackgroundColor = selected ? selectionColor : Constants.Color.LightGray;
                iconImageView.Image = selected ? selectedIconImage : iconImage;
            });
        }
    }
}
