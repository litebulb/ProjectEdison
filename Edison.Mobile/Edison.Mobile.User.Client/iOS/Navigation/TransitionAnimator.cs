using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Navigation
{
    public abstract class TransitionAnimator : NSObject, IUIViewControllerAnimatedTransitioning, IUIViewControllerInteractiveTransitioning
    {
        protected UIViewPropertyAnimator animator;
        protected UIGestureRecognizer.Token panToken;
        protected UIPanGestureRecognizer panGestureRecognizer;
        protected IUIViewControllerContextTransitioning transitionContext;
        protected UIView toView;
        protected UIView fromView;
        protected UIViewController fromViewController;
        protected UIViewController toViewController;

        protected readonly float DEFAULT_DURATION = 0.25f;
        protected readonly float VELOCITY_THRESHOLD = 200;

        public UIPanGestureRecognizer PanGestureRecognizer
        {
            get => panGestureRecognizer;
            set
            {
                if (panToken != null) panGestureRecognizer.RemoveTarget(panToken);
                panGestureRecognizer = value;
                panToken = panGestureRecognizer.AddTarget(HandlePan);
            }
        }

        public bool IsPresenting { get; set; }

        public bool WantsInteractiveStart { get; set; }

        public virtual void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            PrepareForTransition(transitionContext);
        }

        public double TransitionDuration(IUIViewControllerContextTransitioning transitionContext) => WantsInteractiveStart ? DEFAULT_DURATION : CreateDefaultAnimator().Duration;

        public virtual void StartInteractiveTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            PrepareForTransition(transitionContext);
        }

        [Export("interruptibleAnimatorForTransition:")]
        public IUIViewImplicitlyAnimating GetInterruptibleAnimator(IUIViewControllerContextTransitioning transitionContext) => animator;

        [Export("animationEnded:")]
        public virtual void AnimationEnded(bool transitionCompleted)
        {
            panGestureRecognizer?.RemoveTarget(panToken);
            panGestureRecognizer = null;
            toView = null;
            toViewController = null;
            fromView = null;
            fromViewController = null;
        }

        protected UIViewPropertyAnimator CreateDefaultAnimator(bool extraDamp = false) => new UIViewPropertyAnimator(0, new UISpringTimingParameters(4.5f, 1300, extraDamp ? 155 : 125, new CGVector(0, 0)));

        protected virtual void HandlePan(NSObject obj) { }

        void PrepareForTransition(IUIViewControllerContextTransitioning context)
        {
            transitionContext = context;
            toView = context.GetViewFor(UITransitionContext.ToViewKey);
            toViewController = context.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);
            fromView = context.GetViewFor(UITransitionContext.FromViewKey);
            fromViewController = context.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
        }
    }
}
