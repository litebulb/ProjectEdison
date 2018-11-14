using System;
using Edison.Core.Common.Models;
using Edison.Mobile.User.Client.iOS.Shared;
using UIKit;
using Edison.Mobile.User.Client.Core.Chat;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class ChatMessageCollectionViewCell : UICollectionViewCell
    {
        readonly nfloat padding = 8;
        readonly nfloat speakerCircleSize = 32;
        readonly nfloat smallerCircleSize = 24;

        bool isInitialized;
        UILabel messageLabel;
        UIView messageBackgroundView;
        CircleAvatarView speakerAvatarView;

        UIView topView;
        UIView bottomView;

        NSLayoutConstraint circleImageViewRightAnchorOutgoingConstraint;
        NSLayoutConstraint circleImageViewLeftAnchorIncomingConstraint;

        NSLayoutConstraint messageLayoutGuideRightAnchorOutgoingConstraint;
        NSLayoutConstraint messageLayoutGuideRightAnchorIncomingConstraint;
        NSLayoutConstraint messageLayoutGuideLeftAnchorOutgoingConstraint;
        NSLayoutConstraint messageLayoutGuideLeftAnchorIncomingConstraint;

        NSLayoutConstraint messageLabelRightAnchorOutgoingConstraint;
        NSLayoutConstraint messageLabelLeftAnchorIncomingConstraint;

        NSLayoutConstraint[] outgoingMessageConstraints;
        NSLayoutConstraint[] incomingMessageConstraints;

        NSLayoutConstraint topViewHeightConstraint;
        NSLayoutConstraint bottomViewHeightConstraint;

        NSLayoutConstraint messageLabelTopAnchorConstraint;
        NSLayoutConstraint bottomViewTopAnchorConstraint;

        NSLayoutConstraint bottomViewRightAnchorConstraint;
        NSLayoutConstraint topViewRightAnchorConstraint;

        public ChatMessageCollectionViewCell(IntPtr handle) : base(handle)
        {
        }

        public void Initialize(ChatMessage message, ChatMessageType suggestionType = null, string initials = null)
        {
            if (!isInitialized)
            {
                isInitialized = true;

                ContentView.TranslatesAutoresizingMaskIntoConstraints = false;
                ContentView.WidthAnchor.ConstraintEqualTo(UIScreen.MainScreen.Bounds.Width).Active = true;
                ContentView.HeightAnchor.ConstraintGreaterThanOrEqualTo(speakerCircleSize + (2 * padding)).Active = true;

                speakerAvatarView = new CircleAvatarView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = Constants.Color.LightGray };
                ContentView.AddSubview(speakerAvatarView);
                speakerAvatarView.WidthAnchor.ConstraintEqualTo(speakerCircleSize).Active = true;
                speakerAvatarView.HeightAnchor.ConstraintEqualTo(speakerCircleSize).Active = true;
                speakerAvatarView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor, constant: padding).Active = true;

                circleImageViewRightAnchorOutgoingConstraint = speakerAvatarView.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor, constant: -padding);
                circleImageViewLeftAnchorIncomingConstraint = speakerAvatarView.LeftAnchor.ConstraintEqualTo(ContentView.LeftAnchor, constant: padding);

                var messageLayoutGuide = new UILayoutGuide();
                ContentView.AddLayoutGuide(messageLayoutGuide);
                messageLayoutGuideRightAnchorOutgoingConstraint = messageLayoutGuide.RightAnchor.ConstraintEqualTo(speakerAvatarView.LeftAnchor, constant: -padding);
                messageLayoutGuideLeftAnchorIncomingConstraint = messageLayoutGuide.LeftAnchor.ConstraintEqualTo(speakerAvatarView.RightAnchor, constant: padding);
                messageLayoutGuideLeftAnchorOutgoingConstraint = messageLayoutGuide.LeftAnchor.ConstraintEqualTo(ContentView.LeftAnchor, constant: padding);
                messageLayoutGuideRightAnchorIncomingConstraint = messageLayoutGuide.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor, constant: -padding);

                messageLabel = new UILabel
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Twelve),
                };

                ContentView.AddSubview(messageLabel);

                topView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, ClipsToBounds = true };
                ContentView.AddSubview(topView);
                topView.TopAnchor.ConstraintEqualTo(speakerAvatarView.TopAnchor, constant: padding).Active = true;
                topView.LeftAnchor.ConstraintEqualTo(messageLabel.LeftAnchor).Active = true;
                topViewRightAnchorConstraint = topView.RightAnchor.ConstraintEqualTo(messageLabel.RightAnchor);
                topViewHeightConstraint = topView.HeightAnchor.ConstraintEqualTo(0);
                topViewHeightConstraint.Active = true;

                bottomView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, ClipsToBounds = true };
                ContentView.AddSubview(bottomView);
                bottomViewTopAnchorConstraint = bottomView.TopAnchor.ConstraintEqualTo(messageLabel.BottomAnchor);
                bottomViewTopAnchorConstraint.Active = true;
                bottomView.LeftAnchor.ConstraintEqualTo(messageLabel.LeftAnchor).Active = true;
                bottomViewRightAnchorConstraint = bottomView.RightAnchor.ConstraintEqualTo(messageLabel.RightAnchor);
                bottomView.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, constant: -padding).Active = true;
                bottomViewHeightConstraint = bottomView.HeightAnchor.ConstraintEqualTo(0);
                bottomViewHeightConstraint.Active = true;

                var locationSentImageView = new UIImageView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Image = Constants.Assets.LocationSent,
                };
                bottomView.AddSubview(locationSentImageView);
                locationSentImageView.LeftAnchor.ConstraintEqualTo(bottomView.LeftAnchor).Active = true;
                locationSentImageView.CenterYAnchor.ConstraintEqualTo(bottomView.CenterYAnchor).Active = true;

                var locationSentLabel = new UILabel
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Ten),
                    TextColor = Constants.Color.MidGray,
                    Text = "Location sent",
                };
                bottomView.AddSubview(locationSentLabel);
                locationSentLabel.LeftAnchor.ConstraintEqualTo(locationSentImageView.RightAnchor, constant: padding / 2).Active = true;
                locationSentLabel.CenterYAnchor.ConstraintEqualTo(bottomView.CenterYAnchor).Active = true;
                locationSentLabel.RightAnchor.ConstraintLessThanOrEqualTo(bottomView.RightAnchor).Active = true;

                messageLabelTopAnchorConstraint = messageLabel.TopAnchor.ConstraintEqualTo(topView.BottomAnchor);
                messageLabelTopAnchorConstraint.Active = true;
                messageLabelRightAnchorOutgoingConstraint = messageLabel.RightAnchor.ConstraintEqualTo(messageLayoutGuide.RightAnchor, constant: -padding);
                messageLabelLeftAnchorIncomingConstraint = messageLabel.LeftAnchor.ConstraintEqualTo(messageLayoutGuide.LeftAnchor, constant: padding);
                messageLabel.WidthAnchor.ConstraintLessThanOrEqualTo(messageLayoutGuide.WidthAnchor, multiplier: 0.75f, constant: -(padding * 2)).Active = true;

                messageBackgroundView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = Constants.Color.LightGray };
                ContentView.InsertSubviewBelow(messageBackgroundView, messageLabel);
                messageBackgroundView.TopAnchor.ConstraintEqualTo(topView.TopAnchor, constant: -padding).Active = true;
                messageBackgroundView.LeftAnchor.ConstraintEqualTo(messageLabel.LeftAnchor, constant: -padding).Active = true;
                messageBackgroundView.RightAnchor.ConstraintEqualTo(messageLabel.RightAnchor, constant: padding).Active = true;
                messageBackgroundView.BottomAnchor.ConstraintEqualTo(bottomView.BottomAnchor, constant: padding).Active = true;
                messageBackgroundView.Layer.CornerRadius = 4;

                outgoingMessageConstraints = new NSLayoutConstraint[]
                {
                    circleImageViewRightAnchorOutgoingConstraint,
                    messageLayoutGuideLeftAnchorOutgoingConstraint,
                    messageLayoutGuideRightAnchorOutgoingConstraint,
                    messageLabelRightAnchorOutgoingConstraint,
                };

                incomingMessageConstraints = new NSLayoutConstraint[]
                {
                    circleImageViewLeftAnchorIncomingConstraint,
                    messageLayoutGuideLeftAnchorIncomingConstraint,
                    messageLayoutGuideRightAnchorIncomingConstraint,
                    messageLabelLeftAnchorIncomingConstraint,
                };
            }

            var isOutgoing = message.UserModel.Role == ChatUserRole.Consumer;
            var isNewPrompt = suggestionType != null;
            var topViewHeight = smallerCircleSize;
            var bottomViewheight = 13;

            foreach (var c in outgoingMessageConstraints) { c.Active = isOutgoing; }
            foreach (var c in incomingMessageConstraints) { c.Active = !isOutgoing; }

            messageLabel.Text = message.Text;
            speakerAvatarView.Initials = isOutgoing ? initials : string.Empty;

            topViewHeightConstraint.Constant = isNewPrompt ? topViewHeight : 0;
            bottomViewHeightConstraint.Constant = isNewPrompt ? bottomViewheight : 0;

            bottomViewTopAnchorConstraint.Constant = isNewPrompt ? padding : 0;
            messageLabelTopAnchorConstraint.Constant = isNewPrompt ? padding : 0;

            bottomViewRightAnchorConstraint.Active = isNewPrompt;
            topViewRightAnchorConstraint.Active = isNewPrompt;

            if (isNewPrompt)
            {
                foreach (var v in topView.Subviews) { v.RemoveFromSuperview(); }

                var suggestionCircleView = new CircleAvatarView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Image = suggestionType.SelectedIconImage,
                    BackgroundColor = suggestionType.SelectionColor,
                };

                topView.AddSubview(suggestionCircleView);
                suggestionCircleView.LeftAnchor.ConstraintEqualTo(topView.LeftAnchor).Active = true;
                suggestionCircleView.CenterYAnchor.ConstraintEqualTo(topView.CenterYAnchor).Active = true;
                suggestionCircleView.WidthAnchor.ConstraintEqualTo(smallerCircleSize).Active = true;
                suggestionCircleView.HeightAnchor.ConstraintEqualTo(smallerCircleSize).Active = true;

                var titleLabel = new UILabel
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Text = suggestionType.Title,
                    Font = Constants.Fonts.RubikOfSize(Constants.Fonts.Size.Ten),
                    TextColor = Constants.Color.MidGray,
                };
                topView.AddSubview(titleLabel);
                titleLabel.LeftAnchor.ConstraintEqualTo(suggestionCircleView.RightAnchor, constant: padding / 2).Active = true;
                titleLabel.CenterYAnchor.ConstraintEqualTo(topView.CenterYAnchor).Active = true;
                titleLabel.RightAnchor.ConstraintEqualTo(topView.RightAnchor).Active = true;
            }
        }
    }
}
