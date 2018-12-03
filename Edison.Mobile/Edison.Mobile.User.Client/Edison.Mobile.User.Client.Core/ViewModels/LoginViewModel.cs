using System.Threading.Tasks;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.Common.Notifications;
using Edison.Mobile.Common.Shared;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.User.Client.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        readonly AuthService authService;
        readonly INotificationService notificationService;
        readonly ILocationService locationService;

        bool isInitialAppearance = true;

        public event ViewNotification OnDisplayLogin;
        public event ViewNotification OnNavigateToMainViewModel;
        public event ViewNotification OnAppPermissionsFailed;

        public LoginViewModel(AuthService authService, ILocationService locationService, INotificationService notificationService)
        {
            this.authService = authService;
            this.locationService = locationService;
            this.notificationService = notificationService;
        }

        public override async void ViewAppearing()
        {
            base.ViewAppearing();

            //await locationService.RequestLocationPrivileges();
            //await notificationService.RequestNotificationPrivileges();
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
                    authService.OnAuthChanged += AuthServiceOnAuthChanged;
                    OnDisplayLogin?.Invoke();
                }
            }
        }

        public override void ViewDestroyed()
        {
            base.ViewDestroyed();
            authService.OnAuthChanged -= AuthServiceOnAuthChanged;
        }

        public async Task SignIn()
        {
            await authService.AcquireToken();
        }

        async void AuthServiceOnAuthChanged(object sender, AuthChangedEventArgs e)
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
            var hasNotificationPrivileges = await notificationService.HasNotificationPrivileges();
            var hasLocationPrivileges = await locationService.HasLocationPrivileges();
            var locationSuccess = hasLocationPrivileges;
            var notificationSuccess = hasNotificationPrivileges;
            
            if (!locationSuccess)
            {
                locationSuccess = await locationService.RequestLocationPrivileges();            
            }

            if (!hasNotificationPrivileges)
            {
                notificationSuccess = await notificationService.RequestNotificationPrivileges();
            }
            
            return notificationSuccess && locationSuccess;
        }
    }
}
