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

        public PlatformAuthService(IPublicClientApplication publicClientApplication, ILogger logger)
        {
            this.publicClientApplication = publicClientApplication;
            this.logger = logger;
        }

        public async Task<AuthenticationResult> AcquireTokenAsync()
        {
            Activity currentActivity = null;

            try
            {
                currentActivity = CurrentActivityHelper.GetCurrentActivity();

            }
            catch (Exception ex)
            {
                logger.Log(ex, "Error getting current activity");
            }

            return await publicClientApplication.AcquireTokenAsync(AuthConfig.Scopes, new UIParent(currentActivity));
        }
    }
}
