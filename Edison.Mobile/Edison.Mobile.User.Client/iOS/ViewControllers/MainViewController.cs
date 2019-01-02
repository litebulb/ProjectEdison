using System;
using CoreGraphics;
using Edison.Mobile.iOS.Common.Shared;
using Edison.Mobile.iOS.Common.Views;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.iOS.Navigation;
using Edison.Mobile.User.Client.iOS.Shared;
using Edison.Mobile.User.Client.iOS.Views;
using UIKit;
using UserNotifications;

namespace Edison.Mobile.User.Client.iOS.ViewControllers
{
    public class MainViewController : BaseViewController<MainViewModel>
    {
        nfloat pulloutMaxDestY;
        nfloat pulloutNeutralDestY;
        nfloat pulloutMinDestY;

        bool isPulloutInMinMode = false;

        MainPulloutView mainPulloutView;
        UIViewPropertyAnimator pulloutAnimator;
        UIPanGestureRecognizer pulloutPanGestureRecognizer;
        UITapGestureRecognizer dismissMenuTapGestureRecognizer;

        UIView pulloutBackgroundView;
        UIView menuBackgroundView;
        UIView edgeGestureCaptureView;

        ResponsesViewController responsesViewController;
        MenuViewController menuViewController;
        ChatViewController chatViewController;

        UIPanGestureRecognizer menuPanGestureRecognizer;
        UIViewPropertyAnimator menuAnimator;

        PulloutState pulloutState;
        MenuState menuState;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = Constants.Color.BackgroundGray;

            Constants.MenuRightMargin = View.Bounds.Width / 2.8f;
            Constants.PulloutBottomMargin = View.Bounds.Height * 0.25f;
            Constants.PulloutTopMargin = View.Bounds.Height * (View.Bounds.Height <= 667 ? 0.04f : 0.06f);
            Constants.PulloutMinBottomMargin = View.Bounds.Height * 0.12f;

            pulloutMaxDestY = Constants.PulloutTopMargin;
            pulloutNeutralDestY = View.Frame.Height - Constants.PulloutBottomMargin;
            pulloutMinDestY = View.Frame.Height - Constants.PulloutMinBottomMargin;

            responsesViewController = new ResponsesViewController();
            var responsesNavController = new ResponsesNavigationController(responsesViewController);
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
                BackgroundColor = Constants.Color.Black.ColorWithAlpha(0.5f),
            };

            View.AddSubview(pulloutBackgroundView);

            pulloutBackgroundView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            pulloutBackgroundView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;
            pulloutBackgroundView.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            pulloutBackgroundView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;

            chatViewController = new ChatViewController();
            AddChildViewController(chatViewController);
            mainPulloutView = new MainPulloutView(chatViewController)
            {
                BackgroundColor = Constants.Color.White,
                Frame = new CGRect
                {
                    Size = new CGSize
                    {
                        Width = View.Bounds.Width,
                        Height = View.Bounds.Height * 1.5f,
                    },
                    Location = new CGPoint
                    {
                        X = 0,
                        Y = UIScreen.MainScreen.Bounds.Height - Constants.PulloutBottomMargin,
                    },
                },
            };

            View.AddSubview(mainPulloutView);
            chatViewController.DidMoveToParentViewController(this);

            menuBackgroundView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Alpha = 0,
                BackgroundColor = Constants.Color.Black.ColorWithAlpha(0.5f),
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

            edgeGestureCaptureView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };
            View.AddSubview(edgeGestureCaptureView);
            edgeGestureCaptureView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            edgeGestureCaptureView.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            edgeGestureCaptureView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            edgeGestureCaptureView.WidthAnchor.ConstraintEqualTo(10).Active = true;

            menuPanGestureRecognizer = new UIPanGestureRecognizer(OnMenuPan)
            {
                ShouldBegin = recognizer => pulloutState != PulloutState.Maximized && !responsesViewController.IsShowingDetails,
            };

            pulloutPanGestureRecognizer = new UIPanGestureRecognizer(HandlePulloutPan);
            dismissMenuTapGestureRecognizer = new UITapGestureRecognizer(HandleDimViewTap);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            mainPulloutView.AddGestureRecognizer(pulloutPanGestureRecognizer);
            menuBackgroundView.AddGestureRecognizer(dismissMenuTapGestureRecognizer);

            View.AddGestureRecognizer(menuPanGestureRecognizer);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            mainPulloutView.RemoveGestureRecognizer(pulloutPanGestureRecognizer);
            menuBackgroundView.RemoveGestureRecognizer(dismissMenuTapGestureRecognizer);

            View.RemoveGestureRecognizer(menuPanGestureRecognizer);
        }

        protected override void BindEventHandlers()
        {
            base.BindEventHandlers();

            responsesViewController.OnMenuTapped += OnMenuTapped;
            responsesViewController.OnViewResponseDetails += HandleOnViewResponseDetails;
            responsesViewController.OnDismissResponseDetails += HandleOnDismissResponseDetails;

            mainPulloutView.OnChatPromptActivated += HandleOnChatPromptActivated;
        }

        protected override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();

            responsesViewController.OnMenuTapped -= OnMenuTapped;
            responsesViewController.OnViewResponseDetails -= HandleOnViewResponseDetails;
            responsesViewController.OnDismissResponseDetails -= HandleOnDismissResponseDetails;

            mainPulloutView.OnChatPromptActivated -= HandleOnChatPromptActivated;
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
                        mainPulloutView.PulloutBeganDragging(pulloutState);
                    }
                    break;
                case UIGestureRecognizerState.Changed:
                    {
                        var potentialNewY = mainPulloutView.Frame.Y + translationY;
                        var translationFactor = GetTranslationFactor(potentialNewY);
                        var newY = mainPulloutView.Frame.Y + (translationY * translationFactor);
                        var totalYDistance = (UIScreen.MainScreen.Bounds.Height - Constants.PulloutBottomMargin) - Constants.PulloutTopMargin;
                        var percentMaximized = 1 - ((newY - Constants.PulloutTopMargin) / totalYDistance);

                        mainPulloutView.Frame = new CGRect
                        {
                            Location = new CGPoint
                            {
                                X = mainPulloutView.Frame.X,
                                Y = newY,
                            },
                            Size = mainPulloutView.Frame.Size,
                        };

                        mainPulloutView.SetPercentMaximized(percentMaximized);
                        pulloutBackgroundView.Alpha = percentMaximized;


                        var minimizedTotalDistance = pulloutMinDestY - pulloutNeutralDestY;
                        var minimizedDeltaProgress = newY - pulloutNeutralDestY;
                        minimizedDeltaProgress = minimizedDeltaProgress < 0 ? 0 : minimizedDeltaProgress;
                        var percentMinimized = minimizedDeltaProgress / minimizedTotalDistance;
                        percentMinimized = percentMinimized > 1 ? 1 : (percentMinimized < 0 ? 0 : percentMinimized);
                        mainPulloutView.SetPercentMinimized(percentMinimized);

                        panGestureRecognizer.SetTranslation(CGPoint.Empty, View);
                    }
                    break;
                case UIGestureRecognizerState.Ended:
                    {
                        var y = mainPulloutView.Frame.Y;
                        var velocityY = panGestureRecognizer.VelocityInView(View).Y;
                        var shouldMaximizeByLocation = y <= pulloutMaxDestY || Math.Abs(y - pulloutMaxDestY) < (pulloutNeutralDestY - pulloutMaxDestY) / 2;
                        var shouldMaximizeByVelocity = -velocityY > Constants.PulloutVelocityThreshold;
                        var shouldMinimizeByVelocity = velocityY > Constants.PulloutVelocityThreshold;
                        var shouldMaximize = !shouldMinimizeByVelocity && (shouldMaximizeByLocation || shouldMaximizeByVelocity);
                        var newPulloutState = shouldMaximize ? PulloutState.Maximized : (isPulloutInMinMode ? PulloutState.Minimized : PulloutState.Neutral);
                        var DEST_Y = PulloutDestinationYFromState(newPulloutState);
                        var remainingDistance = Math.Abs(y - DEST_Y);

                        AnimatePullout(newPulloutState, (float)(PulloutIsExceedingBoundaries(mainPulloutView.Frame) ? 0f : (nfloat)Math.Abs(velocityY / remainingDistance)));
                    }
                    break;
            }
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
            var topLimitY = Constants.PulloutTopMargin;
            if (potentialNewY < topLimitY)
            {
                var excessDistance = topLimitY - potentialNewY;
                return 0.3f - (excessDistance / (topLimitY * marginMultiplier));
            }

            var bottomMargin = isPulloutInMinMode ? Constants.PulloutMinBottomMargin : Constants.PulloutBottomMargin;
            var bottomLimitY = View.Bounds.Height - bottomMargin;
            if (potentialNewY > bottomLimitY)
            {
                var excessDistance = potentialNewY - bottomLimitY;
                return 0.3f - (excessDistance / (bottomMargin * marginMultiplier));
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
                return 0.3f - (nfloat)Math.Abs(excessDistance / (limitX * marginMultiplier));
            }

            return 1;
        }

        void AnimatePullout(PulloutState newPulloutState, float initialVelocity = 0.7f, bool launchKeyboard = false)
        {
            var initialVelocityVector = new CGVector(0f, initialVelocity);

            var springParameters = GetSpringTimingParameters(initialVelocityVector);
            var destinationY = PulloutDestinationYFromState(newPulloutState);

            pulloutAnimator = new UIViewPropertyAnimator(0, springParameters)
            {
                Interruptible = true,
            };

            pulloutAnimator.AddAnimations(() =>
            {
                mainPulloutView.Frame = new CGRect
                {
                    Location = new CGPoint
                    {
                        X = mainPulloutView.Frame.X,
                        Y = destinationY,
                    },
                    Size = mainPulloutView.Frame.Size,
                };

                mainPulloutView.SetPercentMaximized(newPulloutState == PulloutState.Maximized ? 1 : 0);
                mainPulloutView.SetPercentMinimized(newPulloutState == PulloutState.Neutral || newPulloutState == PulloutState.Maximized ? 0 : 1);

                pulloutBackgroundView.Alpha = newPulloutState == PulloutState.Maximized ? 1 : 0;
            });

            if (launchKeyboard)
            {
                mainPulloutView.LaunchKeyboard();
            }

            pulloutAnimator.AddCompletion(pos =>
            {
                pulloutState = newPulloutState;
                mainPulloutView.PulloutDidFinishAnimating(newPulloutState);
            });

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
                case PulloutState.Minimized: return pulloutMinDestY;
                default: return pulloutNeutralDestY;
            }
        }

        nfloat MenuDestinationXFromState(MenuState state)
        {
            switch (state)
            {
                case MenuState.Closed: return -UIScreen.MainScreen.Bounds.Width;
                default: return -Constants.MenuRightMargin;
            }
        }

        void HandleOnViewResponseDetails(object sender, EventArgs e)
        {
            isPulloutInMinMode = true;
            AnimatePullout(PulloutState.Minimized);
        }

        void HandleOnDismissResponseDetails(object sender, EventArgs e)
        {
            AnimatePullout(PulloutState.Neutral);
            isPulloutInMinMode = false;
        }

        void HandleOnChatPromptActivated(object sender, Core.Shared.ChatPromptType e)
        {
            if (e == Core.Shared.ChatPromptType.Emergency || e == Core.Shared.ChatPromptType.ReportActivity) 
            {
                AnimatePullout(PulloutState.Maximized, 0.7f, true);
            }
        }

        UISpringTimingParameters GetSpringTimingParameters(CGVector initialVelocity) => new UISpringTimingParameters(4.5f, 900f, 90f, initialVelocity);

        bool PulloutIsExceedingBoundaries(CGRect pulloutFrame) => pulloutFrame.Y < Constants.PulloutTopMargin || pulloutFrame.Y > View.Bounds.Height - Constants.PulloutBottomMargin;

        bool MenuIsExceedingBoundaries(CGRect menuFrame) => menuFrame.X > -Constants.MenuRightMargin || menuFrame.X < -(2 * UIScreen.MainScreen.Bounds.Width);
    }
}
