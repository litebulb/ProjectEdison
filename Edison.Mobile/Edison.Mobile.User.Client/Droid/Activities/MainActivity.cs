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
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.Widget;
using Edison.Mobile.User.Client.Droid.Adapters;
using System.Collections.Generic;

namespace Edison.Mobile.User.Client.Droid.Activities
{
    [Activity]
    public class MainActivity : BaseActivity<MainViewModel>, View.IOnTouchListener
    {
        bool isFirstLayout = true;
        RelativeLayout relativeLayout;
        RelativeLayout pulloutView;
        float? pulloutLastPosY;

        VelocityTracker pulloutVelocityTracker;
        float pulloutBottomMarginDp;
        float pulloutTopMarginDp;
        bool isMenuOpen;

        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        ResponsesAdapter adapter;

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

            adapter = new ResponsesAdapter(this);

            SetContentView(Resource.Layout.main_activity);

            // Get our RecyclerView layout:
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            // Plug the adapter into the RecyclerView:
            recyclerView.SetAdapter(adapter);

            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);

            recyclerView.SetLayoutManager(layoutManager);

            pulloutBottomMarginDp = Constants.PulloutBottomMargin * Resources.DisplayMetrics.Density;
            pulloutTopMarginDp = Constants.PulloutTopMargin * Resources.DisplayMetrics.Density;

            // menuLeftMarginDp = Constants.MenuLeftMargin * Resources.DisplayMetrics.Density;
            // menuRightMarginDp = Constants.MenuRightMargin * Resources.DisplayMetrics.Density;

            relativeLayout = FindViewById<RelativeLayout>(Resource.Id.relativeLayout);
            pulloutView = FindViewById<RelativeLayout>(Resource.Id.pulloutView);
            var pulloutViewLayoutParms = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            pulloutViewLayoutParms.AddRule(LayoutRules.AlignEnd, relativeLayout.Id);
            pulloutView.LayoutParameters = pulloutViewLayoutParms;

            pulloutView.SetOnTouchListener(this);
            pulloutView.BringToFront();

            /* menuView = FindViewById<RelativeLayout>(Resource.Id.menuView);
              var menuViewLayoutParms = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
              menuViewLayoutParms.AddRule(LayoutRules.AlignStart, relativeLayout.Id);
              menuView.LayoutParameters = menuViewLayoutParms;
              menuView.SetOnTouchListener(this);
              relativeLayout.SetOnTouchListener(this);*/

            var menuButton = FindViewById<ImageButton>(Resource.Id.menu_btn);
            menuButton.Click += OpenMenu;
        }

        private void OpenMenu(object sender, EventArgs eventArgs)  { }

        public bool OnTouch(View v, MotionEvent e)
        {
            //Menu
            //var deltaX = (menuLastPosX ?? e.GetX() - e.GetX());
            //var transX = menuView.TranslationX - deltaX;
            //var transRightLimit = Constants.MenuRightMargin * Resources.DisplayMetrics.Density;
            //var transLeftLimit = VisibleDisplayRect.Left - pulloutBottomMarginDp;

            //Pullout
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


