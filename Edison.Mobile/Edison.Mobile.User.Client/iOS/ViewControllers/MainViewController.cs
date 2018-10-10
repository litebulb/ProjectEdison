using System;
using CoreGraphics;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.iOS.Common.Views;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.iOS.Shared;
using Edison.Mobile.User.Client.iOS.Views;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.ViewControllers
{
    public class MainViewController : BaseViewController<MainViewModel>
    {
        nfloat pulloutMaxDestY;
        nfloat pulloutNeutralDestY;

        MainPulloutView pulloutView;
        UIViewPropertyAnimator pulloutAnimator;
        UIPanGestureRecognizer pulloutPanGestureRecognizer;
        UITapGestureRecognizer dismissPulloutTapGestureRecognizer;
        UITapGestureRecognizer dismissMenuTapGestureRecognizer;

        UIView pulloutBackgroundView;
        UIView menuBackgroundView;

        ResponsesViewController responsesViewController;
        MenuViewController menuViewController;
        UIPanGestureRecognizer menuPanGestureRecognizer;
        UIViewPropertyAnimator menuAnimator;

        PulloutState pulloutState;
        MenuState menuState;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = PlatformConstants.Color.BackgroundGray;

            Constants.MenuRightMargin = View.Bounds.Width / 2.8f;

            responsesViewController = new ResponsesViewController();
            var responsesNavController = new UINavigationController(responsesViewController);
            AddChildViewController(responsesNavController);
            responsesNavController.View.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(responsesNavController.View);
            responsesNavController.View.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            responsesNavController.View.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            responsesNavController.View.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            responsesNavController.View.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            responsesNavController.DidMoveToParentViewController(this);

            pulloutBackgroundView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Alpha = 0,
                BackgroundColor = PlatformConstants.Color.Black.ColorWithAlpha(0.5f),
            };

            View.AddSubview(pulloutBackgroundView);

            pulloutBackgroundView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            pulloutBackgroundView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            pulloutBackgroundView.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            pulloutBackgroundView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;

            pulloutView = new MainPulloutView
            {
                BackgroundColor = PlatformConstants.Color.White,
                Frame = new CGRect
                {
                    Size = new CGSize
                    {
                        Width = View.Bounds.Width,
                        Height = View.Bounds.Height * 2,
                    },
                    Location = new CGPoint
                    {
                        X = 0,
                        Y = UIScreen.MainScreen.Bounds.Height - Constants.PulloutBottomMargin,
                    },
                },
            };

            View.AddSubview(pulloutView);

            menuBackgroundView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Alpha = 0,
                BackgroundColor = PlatformConstants.Color.Black.ColorWithAlpha(0.5f),
            };

            View.AddSubview(menuBackgroundView);

            menuBackgroundView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            menuBackgroundView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            menuBackgroundView.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            menuBackgroundView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;

            menuViewController = new MenuViewController();
            AddChildViewController(menuViewController);
            menuViewController.View.Frame = new CGRect
            {
                Size = View.Bounds.Size,
                Location = new CGPoint
                {
                    X = -View.Bounds.Width,
                    Y = 0,
                },
            };
            View.AddSubview(menuViewController.View);
            menuViewController.DidMoveToParentViewController(this);

            menuPanGestureRecognizer = new UIPanGestureRecognizer(OnMenuPan)
            {
                ShouldBegin = recognizer => pulloutState != PulloutState.Maximized && !responsesViewController.IsShowingDetails,
            };

            pulloutMaxDestY = Constants.PulloutTopMargin;
            pulloutNeutralDestY = View.Frame.Height - Constants.PulloutBottomMargin;

            pulloutPanGestureRecognizer = new UIPanGestureRecognizer(HandlePulloutPan);
            dismissPulloutTapGestureRecognizer = new UITapGestureRecognizer(HandleDimViewTap);
            dismissMenuTapGestureRecognizer = new UITapGestureRecognizer(HandleDimViewTap);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            pulloutView.AddGestureRecognizer(pulloutPanGestureRecognizer);
            pulloutBackgroundView.AddGestureRecognizer(dismissPulloutTapGestureRecognizer);
            menuBackgroundView.AddGestureRecognizer(dismissMenuTapGestureRecognizer);

            View.AddGestureRecognizer(menuPanGestureRecognizer);

            responsesViewController.OnMenuTapped += OnMenuTapped;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            pulloutView.RemoveGestureRecognizer(pulloutPanGestureRecognizer);
            pulloutBackgroundView.RemoveGestureRecognizer(dismissPulloutTapGestureRecognizer);
            menuBackgroundView.RemoveGestureRecognizer(dismissMenuTapGestureRecognizer);

            View.RemoveGestureRecognizer(menuPanGestureRecognizer);
            //View.RemoveGestureRecognizer(menuEdgePanGestureRecognizer);

            responsesViewController.OnMenuTapped -= OnMenuTapped;
        }

        void HandleDimViewTap(UITapGestureRecognizer tapGestureRecognizer)
        {
            if (pulloutState == PulloutState.Maximized) AnimatePullout(PulloutState.Neutral);
            else if (menuState == MenuState.Open) AnimateMenu(MenuState.Closed);
        }

        void OnMenuTapped(object sender, EventArgs e)
        {
            AnimateMenu(MenuState.Open);
        }

        void HandlePulloutPan(UIPanGestureRecognizer panGestureRecognizer)
        {
            var translationY = panGestureRecognizer.TranslationInView(View).Y;

            switch (panGestureRecognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    {
                        pulloutAnimator?.StopAnimation(true);
                    }
                    break;
                case UIGestureRecognizerState.Changed:
                    {
                        var potentialNewY = pulloutView.Frame.Y + translationY;
                        var translationFactor = GetTranslationFactor(potentialNewY);
                        var newY = pulloutView.Frame.Y + (translationY * translationFactor);
                        var totalYDistance = (UIScreen.MainScreen.Bounds.Height - Constants.PulloutBottomMargin) - Constants.PulloutTopMargin;
                        var percentMaximized = 1 - ((newY - Constants.PulloutTopMargin) / totalYDistance);

                        pulloutView.Frame = new CGRect
                        {
                            Location = new CGPoint
                            {
                                X = pulloutView.Frame.X,
                                Y = newY,
                            },
                            Size = pulloutView.Frame.Size,
                        };

                        pulloutView.SetPercentMaximized(percentMaximized);
                        pulloutBackgroundView.Alpha = percentMaximized;
                        panGestureRecognizer.SetTranslation(CGPoint.Empty, View);
                    }
                    break;
                case UIGestureRecognizerState.Ended:
                    {
                        var y = pulloutView.Frame.Y;
                        var velocityY = panGestureRecognizer.VelocityInView(View).Y;
                        var shouldMaximizeByLocation = y <= pulloutMaxDestY || Math.Abs(y - pulloutMaxDestY) < (pulloutNeutralDestY - pulloutMaxDestY) / 2;
                        var shouldMaximizeByVelocity = -velocityY > Constants.PulloutVelocityThreshold;
                        var shouldMinimizeByVelocity = velocityY > Constants.PulloutVelocityThreshold;
                        var shouldMaximize = !shouldMinimizeByVelocity && (shouldMaximizeByLocation || shouldMaximizeByVelocity);
                        var newPulloutState = shouldMaximize ? PulloutState.Maximized : PulloutState.Neutral;
                        var DEST_Y = PulloutDestinationYFromState(newPulloutState);
                        var remainingDistance = Math.Abs(y - DEST_Y);

                        AnimatePullout(newPulloutState, (float)(PulloutIsExceedingBoundaries(pulloutView.Frame) ? 0f : (nfloat)Math.Abs(velocityY / remainingDistance)));
                    }
                    break;
            }
        }

        void OnMenuEdgePan(UIScreenEdgePanGestureRecognizer panGestureRecognizer)
        {
            OnMenuPan(panGestureRecognizer);
        }

        void OnMenuPan(UIPanGestureRecognizer panGestureRecognizer)
        {
            var translationX = panGestureRecognizer.TranslationInView(View).X;
            var menuView = menuViewController.View;

            switch (panGestureRecognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    {
                        menuAnimator?.StopAnimation(true);
                    }
                    break;
                case UIGestureRecognizerState.Changed:
                    {
                        var potentialNewX = menuView.Frame.X + translationX;
                        var translationFactor = GetMenuTranslationFactor(potentialNewX);
                        var newX = menuView.Frame.X + (translationX * translationFactor);
                        var totalXDistance = UIScreen.MainScreen.Bounds.Width - Constants.MenuRightMargin;
                        var percentMaximized = (newX + UIScreen.MainScreen.Bounds.Width) / totalXDistance;

                        menuView.Frame = new CGRect
                        {
                            Location = new CGPoint
                            {
                                X = newX,
                                Y = menuView.Frame.Y,
                            },
                            Size = menuView.Frame.Size,
                        };

                        menuViewController.SetPercentMaximized((float)percentMaximized);
                        menuBackgroundView.Alpha = percentMaximized;
                        panGestureRecognizer.SetTranslation(CGPoint.Empty, View);
                    }
                    break;
                case UIGestureRecognizerState.Ended:
                    {
                        var x = menuView.Frame.X;
                        var velocityX = panGestureRecognizer.VelocityInView(View).X;
                        var totalXDistance = UIScreen.MainScreen.Bounds.Width - Constants.MenuRightMargin;
                        var shouldMaximizeByLocation = (x + UIScreen.MainScreen.Bounds.Width) >= totalXDistance / 3f;

                        var shouldMaximizeByVelocity = velocityX > Constants.PulloutVelocityThreshold;
                        var shouldMinimizeByVelocity = -velocityX > Constants.PulloutVelocityThreshold;
                        var shouldMaximize = !shouldMinimizeByVelocity && (shouldMaximizeByLocation || shouldMaximizeByVelocity);

                        var DEST_X = shouldMaximize ? -Constants.MenuRightMargin : -UIScreen.MainScreen.Bounds.Width;
                        var remainingDistance = Math.Abs(x - DEST_X);

                        var initialVelocity = MenuIsExceedingBoundaries(menuView.Frame) ? 0f : (nfloat)Math.Abs(velocityX / remainingDistance);
                        AnimateMenu(shouldMaximize ? MenuState.Open : MenuState.Closed, (float)initialVelocity);
                    }
                    break;
            }
        }

        nfloat GetTranslationFactor(nfloat potentialNewY)
        {
            var marginMultiplier = 3f;
            if (potentialNewY < Constants.PulloutTopMargin)
            {
                var excessDistance = Constants.PulloutTopMargin - potentialNewY;
                return 0.3f - (excessDistance / (Constants.PulloutTopMargin * marginMultiplier));
            }

            if (potentialNewY > View.Bounds.Height - Constants.PulloutBottomMargin)
            {
                var excessDistance = potentialNewY - (View.Bounds.Height - Constants.PulloutBottomMargin);
                return 0.3f - (excessDistance / (Constants.PulloutBottomMargin * marginMultiplier));
            }

            return 1;
        }

        nfloat GetMenuTranslationFactor(nfloat potentialNewX)
        {
            var marginMultiplier = 3f;
            var limitX = -Constants.MenuRightMargin;
            if (potentialNewX > limitX)
            {
                var excessDistance = potentialNewX - limitX;
                return 0.3f - (excessDistance / (limitX * marginMultiplier));
            }

            return 1;
        }

        void AnimatePullout(PulloutState newPulloutState, float initialVelocity = 0.7f)
        {
            var initialVelocityVector = new CGVector(0f, initialVelocity);

            var springParameters = GetSpringTimingParameters(initialVelocityVector);
            var destinationY = PulloutDestinationYFromState(newPulloutState);
            var shouldMaximize = destinationY == pulloutMaxDestY;

            pulloutAnimator = new UIViewPropertyAnimator(0, springParameters)
            {
                Interruptible = true,
            };

            pulloutAnimator.AddAnimations(() =>
            {
                pulloutView.Frame = new CGRect
                {
                    Location = new CGPoint
                    {
                        X = pulloutView.Frame.X,
                        Y = destinationY,
                    },
                    Size = pulloutView.Frame.Size,
                };

                pulloutView.SetPercentMaximized(shouldMaximize ? 1 : 0);
                pulloutBackgroundView.Alpha = shouldMaximize ? 1 : 0;
            });

            pulloutAnimator.AddCompletion(pos => pulloutState = shouldMaximize ? PulloutState.Maximized : PulloutState.Neutral);

            pulloutAnimator.StartAnimation();
        }

        void AnimateMenu(MenuState newMenuState, float initialVelocity = 0.7f)
        {
            var initialVelocityVector = new CGVector(initialVelocity, 0f);
            var springParameters = GetSpringTimingParameters(initialVelocityVector);
            var shouldMaximize = newMenuState == MenuState.Open;

            menuAnimator = new UIViewPropertyAnimator(0, springParameters)
            {
                Interruptible = true,
            };

            menuAnimator.AddAnimations(() =>
            {
                menuViewController.View.Frame = new CGRect
                {
                    Location = new CGPoint
                    {
                        X = MenuDestinationXFromState(newMenuState),
                        Y = 0f,
                    },
                    Size = menuViewController.View.Frame.Size,
                };

                menuViewController.SetPercentMaximized(shouldMaximize ? 1 : 0);
                menuBackgroundView.Alpha = shouldMaximize ? 1 : 0;
            });

            menuAnimator.AddCompletion(pos => menuState = shouldMaximize ? MenuState.Open : MenuState.Closed);

            menuAnimator.StartAnimation();
        }

        nfloat PulloutDestinationYFromState(PulloutState state)
        {
            switch (state)
            {
                case PulloutState.Maximized: return pulloutMaxDestY;
                case PulloutState.Neutral: return pulloutNeutralDestY;
            }

            return pulloutNeutralDestY; // TODO: minimized pullout
        }

        nfloat MenuDestinationXFromState(MenuState state)
        {
            switch (state)
            {
                case MenuState.Closed: return -UIScreen.MainScreen.Bounds.Width;
                default: return -Constants.MenuRightMargin;
            }
        }

        UISpringTimingParameters GetSpringTimingParameters(CGVector initialVelocity) => new UISpringTimingParameters(4.5f, 900f, 90f, initialVelocity);

        bool PulloutIsExceedingBoundaries(CGRect pulloutFrame) => pulloutFrame.Y < Constants.PulloutTopMargin || pulloutFrame.Y > View.Bounds.Height - Constants.PulloutBottomMargin;

        bool MenuIsExceedingBoundaries(CGRect menuFrame) => menuFrame.X > -Constants.MenuRightMargin || menuFrame.X < -(2 * UIScreen.MainScreen.Bounds.Width);
    }
}
