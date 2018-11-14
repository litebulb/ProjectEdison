using System;
using System.Threading.Tasks;
using Edison.Mobile.Common.Auth;
using Microsoft.Identity.Client;

namespace Edison.Mobile.iOS.Common.Auth
{
    public class PlatformAuthService : IPlatformAuthService
    {
        readonly IPublicClientApplication publicClientApplication;

        public PlatformAuthService(IPublicClientApplication publicClientApplication)
        {
            this.publicClientApplication = publicClientApplication;
        }

        public async Task<AuthenticationResult> AcquireTokenAsync() => await publicClientApplication.AcquireTokenAsync(AuthConfig.Scopes, new UIParent());
    }
}
