using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Edison.Mobile.Common.Logging;
using Microsoft.Identity.Client;

namespace Edison.Mobile.Common.Auth
{
    public class AuthService
    {

        public event EventHandler<AuthChangedEventArgs> OnAuthChanged;

        readonly IPlatformAuthService platformAuthService;
        readonly IAppAuthService appAuthService;
        readonly IPublicClientApplication publicClientApplication;
        readonly ILogger logger;


        public UIParent UiParent
        {
            get { return platformAuthService?.UiParent; }
            set
            {
                if (platformAuthService != null)
                    platformAuthService.UiParent = value;
            }
        }

        public AuthenticationResult AuthenticationResult { get; set; }

        public UserInfo UserInfo { get; private set; }

        public string Initials => $"{UserInfo?.GivenName?.Substring(0, 1) ?? ""}{UserInfo?.FamilyName?.Substring(0, 1) ?? ""}";

        public AuthService(IPlatformAuthService platformAuthService, IAppAuthService appAuthService, IPublicClientApplication publicClientApplication, ILogger logger)
        {
            this.platformAuthService = platformAuthService;
            this.appAuthService = appAuthService;
            this.publicClientApplication = publicClientApplication;
            this.logger = logger;
        }

        public async Task<bool> AcquireToken()
        {
            try
            {
                AuthenticationResult = await platformAuthService.AcquireTokenAsync();

                if (AuthenticationResult != null && AuthenticationResult.IdToken != null)
                {
                    HandleTokenAcquisition(false);
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

                HandleTokenAcquisition(authenticationResult != null);

                return authenticationResult != null;
            }
            catch (Exception ex)
            {
                logger.Log(ex, "Error acquiring B2C token silently");
            }   

            return false;
        }

        public async Task SignOut()
        {
            var accounts = await publicClientApplication.GetAccountsAsync();
            foreach (var account in accounts)
            {
                await publicClientApplication.RemoveAsync(account);
            }
        }

        void HandleTokenAcquisition(bool wasAcquiredSilently)
        {
            UserInfo = appAuthService.GetUserInfo(AuthenticationResult.IdToken);
            OnAuthChanged?.Invoke(this, new AuthChangedEventArgs
            {
                IsLoggedIn = AuthenticationResult != null,
                WasTokenAcquiredSilently = wasAcquiredSilently,
            });
        }
    }
}
