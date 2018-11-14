using Edison.Core.Common.Models;
using Edison.Mobile.User.Client.iOS.Shared;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class ResponseDetailsView : UIView
    {
        readonly UILabel titleLabel;
        readonly UIView logoImageBackground;
        readonly UIImageView logoImageView;
        readonly UILabel notificationLabel;
        readonly UILabel contentLabel;
        readonly UIButton moreInfoButton;

        ResponseModel response;

        public ResponseModel Response
        {
            get => response;
            set
            {
                response = value;
                if (response != null) UpdateView();
            }
        }

        public ResponseDetailsView()
        {
            BackgroundColor = Constants.Color.White;

            var topBarVerticalPadding = 6f;
            var topBarView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };

            AddSubview(topBarView);
            topBarView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            topBarView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            topBarView.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            topBarView.HeightAnchor.ConstraintEqualTo(HeightAnchor, multiplier: 0.24f).Active = true;

            logoImageBackground = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = Constants.Color.LightGray,
            };

            topBarView.AddSubview(logoImageBackground);
            logoImageBackground.LeftAnchor.ConstraintEqualTo(topBarView.LeftAnchor, Constants.Padding).Active = true;
            logoImageBackground.CenterYAnchor.ConstraintEqualTo(topBarView.CenterYAnchor).Active = true;
            logoImageBackground.HeightAnchor.ConstraintEqualTo(topBarView.HeightAnchor, multiplier: 0.723f).Active = true;
            logoImageBackground.WidthAnchor.ConstraintEqualTo(logoImageBackground.HeightAnchor).Active = true;

            logoImageView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ContentMode = UIViewContentMode.ScaleAspectFit,
            };

            topBarView.AddSubview(logoImageView);
            logoImageView.CenterXAnchor.ConstraintEqualTo(logoImageBackground.CenterXAnchor).Active = true;
            logoImageView.CenterYAnchor.ConstraintEqualTo(logoImageBackground.CenterYAnchor).Active = true;
            logoImageView.HeightAnchor.ConstraintEqualTo(logoImageBackground.HeightAnchor, multiplier: 0.65f).Active = true;
            logoImageView.WidthAnchor.ConstraintEqualTo(logoImageBackground.WidthAnchor, multiplier: 0.65f).Active = true;

            titleLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = Constants.Fonts.RubikMediumOfSize(Constants.Fonts.Size.Fourteen),
                TextColor = Constants.Color.DarkGray,
            };

            topBarView.AddSubview(titleLabel);
            titleLabel.LeftAnchor.ConstraintEqualTo(logoImageBackground.RightAnchor, 8f).Active = true;
            titleLabel.RightAnchor.ConstraintEqualTo(topBarView.RightAnchor, -Constants.Padding).Active = true;
            titleLabel.CenterYAnchor.ConstraintEqualTo(topBarView.CenterYAnchor).Active = true;

            notificationLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Eight),
                TextColor = Constants.Color.MidGray,
            };

            AddSubview(notificationLabel);
            notificationLabel.LeftAnchor.ConstraintEqualTo(LeftAnchor, Constants.Padding).Active = true;
            notificationLabel.TopAnchor.ConstraintEqualTo(topBarView.BottomAnchor, 3f).Active = true;
            notificationLabel.RightAnchor.ConstraintEqualTo(RightAnchor, -Constants.Padding).Active = true;
            notificationLabel.SetContentHuggingPriority(1000, UILayoutConstraintAxis.Vertical);

            moreInfoButton = new UIButton { TranslatesAutoresizingMaskIntoConstraints = false, UserInteractionEnabled = false };
            moreInfoButton.SetTitle("MORE INFO", UIControlState.Normal);
            moreInfoButton.TitleLabel.Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Fourteen);
            moreInfoButton.SetTitleColor(Constants.Color.Blue, UIControlState.Normal);

            AddSubview(moreInfoButton);
            moreInfoButton.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            moreInfoButton.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            moreInfoButton.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            moreInfoButton.HeightAnchor.ConstraintEqualTo(34).Active = true;

            contentLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Font = Constants.Fonts.RubikMediumOfSize(Constants.Fonts.Size.Ten),
                TextColor = Constants.Color.DarkGray,
            };

            AddSubview(contentLabel);
            contentLabel.LeftAnchor.ConstraintEqualTo(LeftAnchor, Constants.Padding).Active = true;
            contentLabel.RightAnchor.ConstraintEqualTo(RightAnchor, -Constants.Padding).Active = true;
            contentLabel.TopAnchor.ConstraintEqualTo(notificationLabel.BottomAnchor, topBarVerticalPadding).Active = true;
            contentLabel.BottomAnchor.ConstraintLessThanOrEqualTo(moreInfoButton.TopAnchor, -topBarVerticalPadding).Active = true;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            logoImageBackground.Layer.CornerRadius = logoImageBackground.Bounds.Height / 2;
        }

        void UpdateView()
        {
            titleLabel.Text = Response.ActionPlan.Name;
            contentLabel.Text = Response.ActionPlan.Description;
            notificationLabel.Text = Response.StartDate.ToString();

            logoImageView.Image = Constants.Assets.MapFromActionPlanIcon(Response.ActionPlan.Icon);
            logoImageBackground.BackgroundColor = Constants.Color.MapFromActionPlanColor(Response.ActionPlan.Color);
        }
    }
}
