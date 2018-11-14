using System;
using CoreGraphics;
using Edison.Mobile.iOS.Common.Shared;
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

        public MainPulloutView(ChatViewController chatViewController)
        {
           this.chatViewController = chatViewController;

            Layer.MasksToBounds = false;
            Layer.ShadowColor = Constants.Color.Black.CGColor;
            Layer.ShadowOffset = new CGSize(2, 2);
            Layer.ShadowOpacity = 0.75f;
            Layer.ShadowRadius = 5;
            Layer.CornerRadius = Constants.CornerRadius;

            indicatorBarView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = Constants.Color.BackgroundGray,
            };

            indicatorBarView.Layer.MasksToBounds = false;
            indicatorBarView.Layer.CornerRadius = barSize.Height / 2;

            AddSubview(indicatorBarView);
            indicatorBarView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
            indicatorBarView.TopAnchor.ConstraintEqualTo(TopAnchor, (Constants.PulloutTopBarHeight / 2) - (barSize.Height / 2)).Active = true;
            indicatorBarView.WidthAnchor.ConstraintEqualTo(barSize.Width).Active = true;
            indicatorBarView.HeightAnchor.ConstraintEqualTo(barSize.Height).Active = true;

            pulloutNeutralView = new PulloutNeutralView { TranslatesAutoresizingMaskIntoConstraints = false };
            AddSubview(pulloutNeutralView);
            pulloutNeutralView.TopAnchor.ConstraintEqualTo(TopAnchor, Constants.PulloutTopBarHeight).Active = true;
            pulloutNeutralView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            pulloutNeutralView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            pulloutNeutralView.HeightAnchor.ConstraintEqualTo(Constants.PulloutBottomMargin - Constants.PulloutTopBarHeight).Active = true;

            chatViewController.View.TranslatesAutoresizingMaskIntoConstraints = false;
            chatViewController.View.Alpha = 0;
            AddSubview(chatViewController.View);
            chatViewController.View.TopAnchor.ConstraintEqualTo(TopAnchor, constant: Constants.PulloutTopBarHeight).Active = true;
            chatViewController.View.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            chatViewController.View.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            chatViewController.View.HeightAnchor.ConstraintEqualTo(UIScreen.MainScreen.Bounds.Height - Constants.PulloutTopMargin - Constants.PulloutTopBarHeight).Active = true;
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
    }
}
