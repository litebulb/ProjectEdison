using Edison.Mobile.Common.Ioc;
using Edison.Mobile.iOS.Common.Ioc;
using Edison.Mobile.User.Client.Core.Ioc;
using Edison.Mobile.User.Client.iOS.Ioc;
using Edison.Mobile.User.Client.iOS.ViewControllers;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Identity.Client;
using UIKit;

namespace Edison.Mobile.User.Client.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Container.Initialize(new CoreContainerRegistrar(), new PlatformCommonContainerRegistrar(), new PlatformContainerRegistrar());

            AppCenter.Start("204ed34e-1e8d-4d9d-a42c-5ef43c492c20", typeof(Analytics), typeof(Crashes));
         
            Window = new UIWindow(UIScreen.MainScreen.Bounds)
            {
                RootViewController = new LoginViewController(),
            };

            Window.MakeKeyAndVisible();

            return true;
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
            return true;
        }
    }
}

