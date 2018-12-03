using System.Linq;
using Autofac;
using Edison.Core.Common.Models;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.Notifications;
using Edison.Mobile.Common.Shared;
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
using WindowsAzure.Messaging;

using SharedConstants = Edison.Mobile.User.Client.Core.Shared.Constants;

namespace Edison.Mobile.User.Client.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        SBNotificationHub Hub { get; set; }

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
            if (!AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url))
            {
                return false;
            }

            return true;
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Hub = new SBNotificationHub(SharedConstants.ListenConnectionString, SharedConstants.NotificationHubName);

            Hub.UnregisterAllAsync(deviceToken, async (error) =>
            {
                if (error != null)
                {
                    System.Diagnostics.Debug.WriteLine("Error calling Unregister: {0}", error.ToString());
                    return;
                }

                var notificationService = Container.Instance.Resolve<INotificationService>();
                var authService = Container.Instance.Resolve<AuthService>();
                var deviceTokenHexString = string.Concat(deviceToken.ToArray().Select(b => b.ToString("X2")));
                await notificationService.RegisterForNotifications(new DeviceRegistrationModel(deviceTokenHexString, NotificationPlatform.APNS, authService.Email));
            });
        }

        void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
        {
            // Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
            if (null != options && options.ContainsKey(new NSString("aps")))
            {
                //Get the aps dictionary
                NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

                string alert = string.Empty;

                //Extract the alert text
                // NOTE: If you're using the simple alert by just specifying
                // "  aps:{alert:"alert msg here"}  ", this will work fine.
                // But if you're using a complex alert with Localization keys, etc.,
                // your "alert" object from the aps dictionary will be another NSDictionary.
                // Basically the JSON gets dumped right into a NSDictionary,
                // so keep that in mind.
                if (aps.ContainsKey(new NSString("alert")))
                    alert = (aps[new NSString("alert")] as NSString).ToString();

                //If this came from the ReceivedRemoteNotification while the app was running,
                // we of course need to manually process things like the sound, badge, and alert.
                if (!fromFinishedLaunching)
                {
                    //Manually show an alert
                    if (!string.IsNullOrEmpty(alert))
                    {
                        var alertController = UIAlertController.Create("Notificiation", alert, UIAlertControllerStyle.Alert);
                        Window?.RootViewController.PresentViewController(alertController, true, null);
                    }
                }
            }
        }
    }
}

