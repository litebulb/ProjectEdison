using System.Threading.Tasks;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Shared;

namespace Edison.Mobile.User.Client.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        readonly AuthService authService;

        bool IsInitialAppearance = true;

        public event ViewNotification OnDisplayLogin;
        public event ViewNotification OnNavigateToMainViewModel;

        public LoginViewModel(AuthService authService)
        {
            this.authService = authService;
        }

        public override async void ViewAppeared()
        {
            base.ViewAppeared();

            if (IsInitialAppearance)
            {
                var hasToken = await authService.AcquireTokenSilently();
                if (hasToken)
                {
                    OnNavigateToMainViewModel?.Invoke();
                }
                else
                {
                    authService.OnAuthChanged += AuthServiceOnAuthChanged;
                    OnDisplayLogin?.Invoke();
                }

                IsInitialAppearance = false;
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

        void AuthServiceOnAuthChanged(object sender, AuthChangedEventArgs e)
        {
            if (e.IsLoggedIn)
            {
                OnNavigateToMainViewModel?.Invoke();
            }
            else
            {
                OnDisplayLogin?.Invoke();
            }
        }
    }
}
