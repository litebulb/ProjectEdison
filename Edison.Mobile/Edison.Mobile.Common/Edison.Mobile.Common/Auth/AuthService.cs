using System;
using System.Linq;
using System.Threading.Tasks;
using Edison.Mobile.Common.Logging;
using Microsoft.Identity.Client;

namespace Edison.Mobile.Common.Auth
{
    public class AuthService
    {
        readonly IPlatformAuthService platformAuthService;
        readonly IPublicClientApplication publicClientApplication;
        readonly ILogger logger;

        public event EventHandler<AuthChangedEventArgs> OnAuthChanged;

        public AuthenticationResult AuthenticationResult { get; set; }

        public AuthService(IPlatformAuthService platformAuthService, IPublicClientApplication publicClientApplication, ILogger logger)
        {
            this.platformAuthService = platformAuthService;
            this.publicClientApplication = publicClientApplication;
            this.logger = logger;
        }

        public async Task<bool> AcquireToken()
        {
            try
            {
                AuthenticationResult = await platformAuthService.AcquireTokenAsync();

                if (AuthenticationResult != null)
                {
                    OnAuthChanged?.Invoke(this, new AuthChangedEventArgs
                    {
                        IsLoggedIn = AuthenticationResult != null,
                        WasTokenAcquiredSilently = false,
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex, "Error acquiring B2C token");
            }

            return AuthenticationResult != null;
        }

        public async Task<bool> AcquireTokenSilently()
        {
            try
            {
                var accounts = await publicClientApplication.GetAccountsAsync();
                var firstAccount = accounts.FirstOrDefault();
                var authenticationResult = await publicClientApplication.AcquireTokenSilentAsync(AuthConfig.Scopes, firstAccount);

                AuthenticationResult = authenticationResult;

                OnAuthChanged?.Invoke(this, new AuthChangedEventArgs
                {
                    IsLoggedIn = authenticationResult != null,
                    WasTokenAcquiredSilently = authenticationResult != null,
                });

                return authenticationResult != null;
            }
            catch (Exception ex)
            {
                logger.Log(ex, "Error acquiring B2C token silently");
            }

            return false;
        }
    }
}
