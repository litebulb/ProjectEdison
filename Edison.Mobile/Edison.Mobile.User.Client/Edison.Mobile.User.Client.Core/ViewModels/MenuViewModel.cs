using System;
using System.Threading.Tasks;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.User.Client.Core.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        readonly AuthService authService;

        public string ProfileName => !(string.IsNullOrEmpty(authService.UserInfo?.GivenName) || string.IsNullOrEmpty(authService.UserInfo?.FamilyName))
            ? $"{authService.UserInfo?.GivenName} {authService.UserInfo?.FamilyName}"
            : authService.UserInfo?.Email;

        public string Initials => authService.Initials;

        public string Email => authService.UserInfo?.Email;

        //TODO  Currently not supported, so just returning null
        public Uri ProfileImageUri => null;  // authService.ProfileImageUri or add from local file system- - needs to be added

        public MenuViewModel(AuthService authService)
        {
            this.authService = authService;
        }

        public async Task SignOut()
        {

            await authService.SignOut();
        }
    }
}
