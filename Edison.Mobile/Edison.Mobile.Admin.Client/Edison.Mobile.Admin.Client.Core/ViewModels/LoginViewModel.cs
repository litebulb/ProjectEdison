using System;
using System.Threading.Tasks;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.Common.Shared;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        readonly ILocationService locationService;
        readonly AuthService authService;

        bool isInitialAppearance = true;

        public event ViewNotification OnDisplayLogin;
        public event ViewNotification OnNavigateToMainViewModel;
        public event ViewNotification OnAppPermissionsFailed;

        public LoginViewModel(AuthService authService, ILocationService locationService)
        {
            this.authService = authService;
            this.locationService = locationService;
        }

        public override async void ViewAppearing()
        {
            base.ViewAppearing();

            //await locationService.RequestLocationPrivileges();
        }

        public override async void ViewAppeared()
        {
            base.ViewAppeared();

            if (isInitialAppearance)
            {
                isInitialAppearance = false;
                var hasToken = await authService.AcquireTokenSilently();
                if (hasToken)
                {
                    await HandleAppPermissions();
                }
                else
                {
                    authService.OnAuthChanged += HandleOnAuthChanged;
                    OnDisplayLogin?.Invoke();
                }
            }
        }

        public override void ViewDestroyed()
        {
            base.ViewDestroyed();
            authService.OnAuthChanged -= HandleOnAuthChanged;
        }

        public async Task SignIn()
        {
            await authService.AcquireToken();
        }

        async void HandleOnAuthChanged(object sender, AuthChangedEventArgs e)
        {
            if (e.IsLoggedIn)
            {
                await HandleAppPermissions();
            }
            else
            {
                OnDisplayLogin?.Invoke();
            }
        }

        async Task HandleAppPermissions()
        {
            var gotPermissions = await GetAppPermissions();
            if (gotPermissions)
            {
                await locationService.StartLocationUpdates();
                OnNavigateToMainViewModel?.Invoke();
            }
            else
            {
                OnAppPermissionsFailed?.Invoke();
            }
        }

        async Task<bool> GetAppPermissions()
        {
            var hasLocationPrivileges = await locationService.HasLocationPrivileges();
            var locationSuccess = hasLocationPrivileges;

            if (!locationSuccess)
            {
                locationSuccess = await locationService.RequestLocationPrivileges();
            }

            return locationSuccess;
        }
    }
}
