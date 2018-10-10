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
        readonly PulloutMinimizedView pulloutMinimizedView;
        readonly PulloutMaximizedView pulloutMaximizedView;

        public MainPulloutView()
        {
            Layer.MasksToBounds = false;
            Layer.ShadowColor = PlatformConstants.Color.Black.CGColor;
            Layer.ShadowOffset = new CGSize(2, 2);
            Layer.ShadowOpacity = 0.75f;
            Layer.ShadowRadius = 5;
            Layer.CornerRadius = Constants.CornerRadius;

            indicatorBarView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = PlatformConstants.Color.BackgroundGray,
            };

            indicatorBarView.Layer.MasksToBounds = false;
            indicatorBarView.Layer.CornerRadius = barSize.Height / 2;

            AddSubview(indicatorBarView);
            indicatorBarView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
            indicatorBarView.TopAnchor.ConstraintEqualTo(TopAnchor, (Constants.PulloutTopBarHeight / 2) - (barSize.Height / 2)).Active = true;
            indicatorBarView.WidthAnchor.ConstraintEqualTo(barSize.Width).Active = true;
            indicatorBarView.HeightAnchor.ConstraintEqualTo(barSize.Height).Active = true;


            pulloutMinimizedView = new PulloutMinimizedView { TranslatesAutoresizingMaskIntoConstraints = false };
            AddSubview(pulloutMinimizedView);
            pulloutMinimizedView.TopAnchor.ConstraintEqualTo(TopAnchor, Constants.PulloutTopBarHeight).Active = true;
            pulloutMinimizedView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            pulloutMinimizedView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            pulloutMinimizedView.HeightAnchor.ConstraintEqualTo(Constants.PulloutBottomMargin - Constants.PulloutTopBarHeight).Active = true;

            pulloutMaximizedView = new PulloutMaximizedView 
            { 
                TranslatesAutoresizingMaskIntoConstraints = false,
                Alpha = 0,
            };

            AddSubview(pulloutMaximizedView);
            pulloutMaximizedView.TopAnchor.ConstraintEqualTo(TopAnchor, Constants.PulloutTopBarHeight).Active = true;
            pulloutMaximizedView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            pulloutMaximizedView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            pulloutMaximizedView.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
        }

        public void SetPercentMaximized(nfloat maximizedPercent)
        {
            pulloutMinimizedView.Alpha = 1 - maximizedPercent;
            pulloutMaximizedView.Alpha = maximizedPercent;
        }
    }
}
