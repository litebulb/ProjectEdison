using System;
using System.Threading.Tasks;
using Android.App;
using Edison.Mobile.Android.Common.Shared;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Microsoft.Identity.Client;

namespace Edison.Mobile.Android.Common.Auth
{
    public class PlatformAuthService : IPlatformAuthService
    {
        readonly IPublicClientApplication publicClientApplication;
        readonly ILogger logger;

        public UIParent UiParent { get; set; } = null;


        public PlatformAuthService(IPublicClientApplication publicClientApplication, ILogger logger)
        {
            this.publicClientApplication = publicClientApplication;
            this.logger = logger;
        }

        public async Task<AuthenticationResult> AcquireTokenAsync()
        {
            if (UiParent == null)
            {
                Activity currentActivity = null;

                try
                {
                    currentActivity = CurrentActivityHelper.GetCurrentActivity();
                    UiParent = new UIParent(currentActivity);
                }
                catch (Exception ex)
                {
                    logger.Log(ex, "Error getting current activity");
                }
            }
            AuthenticationResult res = null;
            try
            {
                res = await publicClientApplication.AcquireTokenAsync(AuthConfig.Scopes, UiParent);
            }
            catch (Exception ex1)
            {
                logger.Log(ex1, "Error aquiring token");
            }
            return res;
        }
    }
}
