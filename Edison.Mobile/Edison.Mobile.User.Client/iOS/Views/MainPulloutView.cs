using System;
using CoreGraphics;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.User.Client.Core.Shared;
using Edison.Mobile.User.Client.iOS.Shared;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Views
{
    public class MainPulloutView : UIView
    {
        readonly UIView indicatorBarView;
        readonly CGSize barSize = new CGSize(80, 8);
        readonly PulloutNeutralView pulloutNeutralView;
        readonly ChatViewController chatViewController;

        PulloutState prevPulloutState = PulloutState.Neutral;

        public event EventHandler<ChatPromptType> OnChatPromptActivated;

        public MainPulloutView(ChatViewController chatViewController)
        {
           this.chatViewController = chatViewController;

            Layer.MasksToBounds = false;
            Layer.ShadowColor = Shared.Constants.Color.Black.CGColor;
            Layer.ShadowOffset = new CGSize(2, 2);
            Layer.ShadowOpacity = 0.75f;
            Layer.ShadowRadius = 5;
            Layer.CornerRadius = Shared.Constants.CornerRadius;

            indicatorBarView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = Shared.Constants.Color.BackgroundGray,
            };

            indicatorBarView.Layer.MasksToBounds = false;
            indicatorBarView.Layer.CornerRadius = barSize.Height / 2;

            AddSubview(indicatorBarView);
            indicatorBarView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
            indicatorBarView.TopAnchor.ConstraintEqualTo(TopAnchor, (Shared.Constants.PulloutTopBarHeight / 2) - (barSize.Height / 2)).Active = true;
            indicatorBarView.WidthAnchor.ConstraintEqualTo(barSize.Width).Active = true;
            indicatorBarView.HeightAnchor.ConstraintEqualTo(barSize.Height).Active = true;

            pulloutNeutralView = new PulloutNeutralView(chatViewController.ViewModel) { TranslatesAutoresizingMaskIntoConstraints = false };
            AddSubview(pulloutNeutralView);
            pulloutNeutralView.TopAnchor.ConstraintEqualTo(TopAnchor, Shared.Constants.PulloutTopBarHeight).Active = true;
            pulloutNeutralView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            pulloutNeutralView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            pulloutNeutralView.HeightAnchor.ConstraintEqualTo(Shared.Constants.PulloutBottomMargin - Shared.Constants.PulloutTopBarHeight).Active = true;

            chatViewController.View.TranslatesAutoresizingMaskIntoConstraints = false;
            chatViewController.View.Alpha = 0;
            AddSubview(chatViewController.View);
            chatViewController.View.TopAnchor.ConstraintEqualTo(TopAnchor, constant: Shared.Constants.PulloutTopBarHeight).Active = true;
            chatViewController.View.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            chatViewController.View.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            chatViewController.View.HeightAnchor.ConstraintEqualTo(UIScreen.MainScreen.Bounds.Height - Shared.Constants.PulloutTopMargin - Shared.Constants.PulloutTopBarHeight).Active = true;

            chatViewController.ViewModel.OnChatPromptActivated += HandleOnChatPromptActivated;
        }

        public override void MovedToWindow()
        {
            if (Window == null)
            {
                chatViewController.ViewModel.OnChatPromptActivated -= HandleOnChatPromptActivated;
            }
        }

        public void SetPercentMaximized(nfloat maximizedPercent)
        {
            pulloutNeutralView.Alpha = 1 - maximizedPercent;
            chatViewController.View.Alpha = maximizedPercent;
        }

        public void SetPercentMinimized(nfloat minimizedPercent) 
        {
            pulloutNeutralView.SetPercentMinimized(minimizedPercent);
        }

        public void PulloutDidFinishAnimating(PulloutState newPulloutState) 
        {
            if (prevPulloutState == PulloutState.Maximized && newPulloutState != PulloutState.Maximized) 
            {
                chatViewController.ChatDismissed();
            }
            else if (newPulloutState == PulloutState.Maximized) 
            {
                chatViewController.ChatSummoned();
            }

            prevPulloutState = newPulloutState;
        }

        public void PulloutBeganDragging(PulloutState fromPulloutState) 
        {
            if (fromPulloutState == PulloutState.Maximized)
            {
                chatViewController.ChatDismissing();
            }
        }

        void HandleOnChatPromptActivated(object sender, ChatPromptType chatPromptType)
        {
            OnChatPromptActivated?.Invoke(this, chatPromptType);
        }
    }
}
