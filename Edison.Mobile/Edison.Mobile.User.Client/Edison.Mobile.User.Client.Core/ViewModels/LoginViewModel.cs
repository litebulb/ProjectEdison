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

        public AuthService AuthService { get; private set; }


        readonly INotificationService notificationService;
        readonly ILocationService locationService;

        bool isInitialAppearance = true;

        public event ViewNotification OnDisplayLogin;
        public event ViewNotification OnLoginFailed;
        public event ViewNotification LoginSucceed;
        public event ViewNotification ClearLoginMessages;
        public event ViewNotification OnNavigateToMainViewModel;
        public event ViewNotification OnAppPermissionsFailed;

        public LoginViewModel(AuthService authService, ILocationService locationService, INotificationService notificationService)
        {
            this.AuthService = authService;
            this.locationService = locationService;
            this.notificationService = notificationService;
        }

        public override async void ViewAppearing()
        {
               base.ViewAppearing();

            //await locationService.RequestLocationPrivileges();
            //await notificationService.RequestNotificationPrivileges();
        }
        public override void ViewCreated()
        {
            base.ViewCreated();
            LoginSucceed += OnLoginSucceeded;
        }

        public override async void ViewAppeared()
        {
            base.ViewAppeared();

            if (isInitialAppearance)
            {
                isInitialAppearance = false;
                // Attenpt to authenticate silently - with a previously stored token
                var hasToken = await AuthService.AcquireTokenSilently();
                if (hasToken)
                    // Authenticated, so check for required permisions
                    await HandleAppPermissions();
                else
                {
                    // Hasn't authenticated
                    // Subscribe to the Authentication OnAuthChanged event (fired when authentiction succeeds)
                    AuthService.OnAuthChanged += HandleOnAuthChanged;
                    // Trigger login screen
                    OnDisplayLogin?.Invoke();
                }
            }
        }

        public override void ViewDestroyed()
        {
            // Unsubscribe from the Authentication OnAuthChanged event
            AuthService.OnAuthChanged -= HandleOnAuthChanged;
            LoginSucceed += OnLoginSucceeded;
            base.ViewDestroyed();

        }

        //iOS sign in
        public async Task SignIn()
        {
            await AuthService.AcquireToken();
        }

        // Android sign in
        public async Task Login()
        {
            var hasToken = await AuthService.AcquireToken();
            // Trigger clear any messages on login screen
            ClearLoginMessages?.Invoke();
            if (!hasToken)
                // Authenication failed - trigger updates to login screen
                OnLoginFailed?.Invoke();
 //         else Login success handled by permisions service which is called as a result of auth changed event which calls HandleOnAuthChanged

        }


        // Called when the Authentication OnAuthChanged event fires - when authentication with credentials succeeds
        async void HandleOnAuthChanged(object sender, AuthChangedEventArgs e)
        {
            if (e.IsLoggedIn)
                // Authenticated, so check for required permisions
                await HandleAppPermissions();
            else
                // Not authenticated, so update login screen
                OnDisplayLogin?.Invoke();
        }

        // Manages required in-app permissions
        async Task HandleAppPermissions()
        {
            // Check and get app permissions
            var gotPermissions = await GetAppPermissions();
            // If permissions have been granted
            if (gotPermissions)
            {
                // Start location service
                await locationService.StartLocationUpdates();
                // Trigger clear any messages from login screen
                ClearLoginMessages?.Invoke();
                // Trigger navigation
                OnNavigateToMainViewModel?.Invoke();
            }
            else
                // Permissions not granted - trigger updated to login screen
                OnAppPermissionsFailed?.Invoke();
        }

        public 


        async Task<bool> GetAppPermissions()
        {
            var hasNotificationPrivileges = await notificationService.HasNotificationPrivileges();
            var hasLocationPrivileges = await locationService.HasLocationPrivileges();
            var locationSuccess = hasLocationPrivileges;
            var notificationSuccess = hasNotificationPrivileges;
            
            if (!locationSuccess)
                locationSuccess = await locationService.RequestLocationPrivileges();            

            if (!hasNotificationPrivileges)
                notificationSuccess = await notificationService.RequestNotificationPrivileges();
            
            return notificationSuccess && locationSuccess;
        }


        private async void OnLoginSucceeded()
        {
            // Start location service
            await locationService.StartLocationUpdates();
        }

        public void InvokeLoginSucceed()
        {
            LoginSucceed?.Invoke();
        }
    }
}
