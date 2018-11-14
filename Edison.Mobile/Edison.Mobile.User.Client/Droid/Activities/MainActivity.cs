using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Edison.Mobile.Android.Common;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.Droid.Views;
using Edison.Mobile.User.Client.Droid.Shared;
using Android.Support.Animation;
using System;

namespace Edison.Mobile.User.Client.Droid.Activities
{
    [Activity(Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : BaseActivity<MainViewModel>, View.IOnTouchListener
    {
        bool isFirstLayout = true;
        PulloutView pulloutView;
        float? pulloutLastPosY;
        VelocityTracker pulloutVelocityTracker;

        float pulloutBottomMarginDp;
        float pulloutTopMarginDp;

        public override void OnGlobalLayout()
        {
            base.OnGlobalLayout();

            if (VisibleDisplayRect.Top > 0 && isFirstLayout)
            {
                pulloutView.TranslationY = VisibleDisplayRect.Bottom - Constants.PulloutBottomMargin * Resources.DisplayMetrics.Density;
                isFirstLayout = false;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            pulloutBottomMarginDp = Constants.PulloutBottomMargin * Resources.DisplayMetrics.Density;
            pulloutTopMarginDp = Constants.PulloutTopMargin * Resources.DisplayMetrics.Density;

            var relativeLayout = new RelativeLayout(this) { Id = 1 };
            var relativeLayoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            relativeLayout.LayoutParameters = relativeLayoutParams;
            relativeLayout.SetBackgroundColor(Constants.BackgroundColor);
            relativeLayout.Alpha = 0.5f;

            pulloutView = new PulloutView(this);
            var pulloutViewLayoutParms = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            pulloutViewLayoutParms.AddRule(LayoutRules.AlignEnd, relativeLayout.Id);
            pulloutView.LayoutParameters = pulloutViewLayoutParms;

            pulloutView.SetOnTouchListener(this);

            relativeLayout.AddView(pulloutView);

            SetContentView(relativeLayout);
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            var deltaY = (pulloutLastPosY ?? e.GetY()) - e.GetY();
            var transY = pulloutView.TranslationY - deltaY;
            var transTopLimit = Constants.PulloutTopMargin * Resources.DisplayMetrics.Density;
            var transBottomLimit = VisibleDisplayRect.Bottom - pulloutBottomMarginDp;

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    pulloutLastPosY = e.GetY();
                    pulloutVelocityTracker?.Clear();
                    pulloutVelocityTracker = pulloutVelocityTracker ?? VelocityTracker.Obtain();
                    pulloutVelocityTracker.AddMovement(e);
                    return true;
                case MotionEventActions.Move:
                    pulloutVelocityTracker.AddMovement(e);
                    if (transY < transTopLimit)
                    {
                        transY = transTopLimit;
                    }
                    else if (transY > transBottomLimit)
                    {
                        transY = transBottomLimit;
                    }

                    pulloutView.TranslationY = transY;
                    return true;
                case MotionEventActions.Cancel:
                case MotionEventActions.Up:
                    pulloutVelocityTracker.ComputeCurrentVelocity(1000);
                    var index = e.ActionIndex;
                    var totalTransDelta = VisibleDisplayRect.Bottom - pulloutBottomMarginDp - pulloutTopMarginDp;
                    var currentTransYDelta = pulloutView.TranslationY - pulloutTopMarginDp;
                    var shouldMaximizeByLocation = currentTransYDelta < (totalTransDelta / 2);
                    var velocityY = pulloutVelocityTracker.GetYVelocity(e.GetPointerId(index));
                    var shouldMaximizeByVelocity = -velocityY > Constants.PulloutVelocityThreshold;
                    var shouldMinimizeByVelocity = velocityY > Constants.PulloutVelocityThreshold;
                    var shouldMaximize = !shouldMinimizeByVelocity && (shouldMaximizeByLocation || shouldMaximizeByVelocity);

                    var springAnimation = new SpringAnimation(pulloutView, DynamicAnimation.TranslationY, shouldMaximize ? pulloutTopMarginDp : VisibleDisplayRect.Bottom - pulloutBottomMarginDp);
                    springAnimation.SetStartVelocity(Math.Abs(shouldMaximizeByVelocity || shouldMinimizeByVelocity ? velocityY : 0));
                    springAnimation.Start();
                    return true;
                default:
                    return true;
            }
        }
    }
}

