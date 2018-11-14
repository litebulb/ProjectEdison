using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Navigation
{
    public class ResponseDetailsTransitionAnimator : TransitionAnimator
    {
        public override void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            base.AnimateTransition(transitionContext);

            Action animation = null;

            if (IsPresenting)
            {
                toView.Frame = transitionContext.ContainerView.Bounds;
                toView.Transform = CGAffineTransform.MakeTranslation(0, toView.Frame.Size.Height);
                transitionContext.ContainerView.AddSubview(toView);
                animation = () => toView.Transform = CGAffineTransform.MakeIdentity();
            }
            else
            {
                toView.Frame = transitionContext.ContainerView.Bounds;
                transitionContext.ContainerView.InsertSubviewBelow(toView, fromView);
                animation = () => fromView.Transform = CGAffineTransform.MakeTranslation(0, toView.Frame.Size.Height);
            }

            animator = CreateDefaultAnimator();
            animator.AddCompletion(position => transitionContext.CompleteTransition(true));
            animator.AddAnimations(animation);
            animator.StartAnimation();
        }

        public override void StartInteractiveTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            base.StartInteractiveTransition(transitionContext);

            Action animation = null;

            if (IsPresenting)
            {
                toView.Frame = transitionContext.ContainerView.Bounds;
                toView.Transform = CGAffineTransform.MakeTranslation(0, toView.Frame.Size.Height);
                transitionContext.ContainerView.AddSubview(toView);
                animation = () => toView.Transform = CGAffineTransform.MakeIdentity();
            }
            else
            {

            }

            if (WantsInteractiveStart)
            {
                animator = new UIViewPropertyAnimator(DEFAULT_DURATION, UIViewAnimationCurve.Linear, animation);
            }
            else
            {
                animator = CreateDefaultAnimator();
                animator.AddCompletion(position => transitionContext.CompleteTransition(true));
                animator.AddAnimations(animation);
                animator.StartAnimation();
            }
        }
    }
}
