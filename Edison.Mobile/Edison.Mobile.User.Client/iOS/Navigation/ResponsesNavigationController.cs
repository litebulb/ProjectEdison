using Edison.Mobile.User.Client.iOS.ViewControllers;
using Foundation;
using UIKit;

namespace Edison.Mobile.User.Client.iOS.Navigation
{
    public class ResponsesNavigationController : UINavigationController, IUINavigationControllerDelegate
    {
        public ResponsesNavigationController(UIViewController rootViewController) 
            :base(rootViewController) 
        {
            Delegate = this;
        }

        [Export("navigationController:animationControllerForOperation:fromViewController:toViewController:")]
        public IUIViewControllerAnimatedTransitioning GetAnimationControllerForOperation(UINavigationController navigationController, UINavigationControllerOperation operation, UIViewController fromViewController, UIViewController toViewController)
        {
            if ((fromViewController is ResponseDetailsViewController || toViewController is ResponseDetailsViewController) &&
                (operation == UINavigationControllerOperation.Pop || operation == UINavigationControllerOperation.Push))
            {
                return new ResponseDetailsTransitionAnimator
                {
                    IsPresenting = operation == UINavigationControllerOperation.Push,
                    WantsInteractiveStart = false,
                };
            }

            return null;
        }
    }
}
