using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Shared;

namespace Edison.Mobile.User.Client.Core.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        readonly AuthService authService;

        public string ProfileName => authService.AuthenticationResult?.Account.Username;

        public MenuViewModel(AuthService authService)
        {
            this.authService = authService;
        }
    }
}
