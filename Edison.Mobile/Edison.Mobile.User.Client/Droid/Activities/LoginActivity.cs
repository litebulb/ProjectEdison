using System;
using System.Threading.Tasks;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Identity.Client;

using Android.App;
using Android.OS;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android.Support.Constraints;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;

using Edison.Mobile.Android.Common;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.Droid.Ioc;
using Edison.Mobile.User.Client.Core.Ioc;
using Edison.Mobile.Android.Common.Ioc;
using Edison.Mobile.Common.Ioc;





#if DEBUG
using Android.Util;
#endif

namespace Edison.Mobile.User.Client.Droid.Activities
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@mipmap/ic_edison_launcher", Exported = true, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginActivity : BaseActivity<LoginViewModel>
    {
        private ConstraintLayout _loginScreen;
        private AppCompatButton _signInButton;
        private ProgressBar _activityIndicator;
        private AppCompatTextView _splachscreenMessage;

        private const int RequestNotificationPermissionId = 0;
        private const int RequestLocationPermissionId = 1;

#if DEBUG
        public static bool UsingLogon = false;
#endif

        protected override void OnCreate(Bundle bundle)
        {
#if DEBUG
            UsingLogon = true;
# endif

            //Initializatio done in MainApplication
//        Container.Initialize(new CoreContainerRegistrar(), new PlatformCommonContainerRegistrar(this), new PlatformContainerRegistrar());

            base.OnCreate(bundle);

            if (ViewModel != null)
                ((LoginViewModel)ViewModel).AuthService.UiParent = new UIParent(this);



            AppCenter.Start("959d5bd2-9e29-4f17-aed6-68885af8c63d", typeof(Analytics), typeof(Crashes));

            SetContentView(Resource.Layout.screen_login);
            BindResources();
            BindVMEvents();


#if DEBUG
            Log.Debug("ACTIVITY", "**************************************");
            Log.Debug("ACTIVITY", "**************************************");
            Log.Debug("ACTIVITY", "**********  LOGIN ACTIVITY  **********");
            Log.Debug("ACTIVITY", "**************************************");
            Log.Debug("ACTIVITY", "**************************************");
#endif




            Task.Run(async () => {
                await Constants.CalculateUIDimensionsAsync(this);
            });

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnBindVMEvents();
        }

        private void BindResources()
        {
            _loginScreen = FindViewById<ConstraintLayout>(Resource.Id.login_screen);
            _signInButton = FindViewById<AppCompatButton>(Resource.Id.signin_button);
            _activityIndicator = FindViewById<ProgressBar>(Resource.Id.login_activity);
            _splachscreenMessage = FindViewById<AppCompatTextView>(Resource.Id.login_msg);
        }

        private async void NavigateToMainViewModel()
        {
            var intent = new Intent(this, typeof(MainActivity));
            //           intent.AddFlags(ActivityFlags.NoAnimation);
 //           intent.AddFlags(ActivityFlags.ClearTop);
 //           intent.AddFlags(ActivityFlags.NewTask);
 //           intent.AddFlags(ActivityFlags.ClearTask);
            StartActivity(intent);

            // Give the main activity a chance to render before killing this one
            // TODO replace with cool transition
            await Task.Run(async () =>
            {
                await Task.Delay(5000);
                Finish();
            }).ConfigureAwait(false);

        }

        protected void BindVMEvents()
        {
            ViewModel.OnDisplayLogin += SignIn;
            ViewModel.ClearLoginMessages += ClearMessages;
            ViewModel.OnLoginFailed += OnLoginFailed;
            ViewModel.OnNavigateToMainViewModel += NavigateToMainViewModel;
            _signInButton.Click += OnButtonClick;
        }

        protected void UnBindVMEvents()
        {
            ViewModel.OnDisplayLogin -= SignIn;
            ViewModel.ClearLoginMessages -= ClearMessages;
            ViewModel.OnLoginFailed -= OnLoginFailed;
            ViewModel.OnNavigateToMainViewModel -= NavigateToMainViewModel;
            _signInButton.Click -= OnButtonClick;
        }

        private async void SignIn()
        {
            await ViewModel.Login();
        }

        // Clears progress bar and messages from the login screen
        private void ClearMessages()
        {
            _activityIndicator.Visibility = ViewStates.Invisible;
            _splachscreenMessage.Visibility = ViewStates.Invisible;
            _signInButton.Visibility = ViewStates.Invisible;
            _loginScreen.Invalidate();
        }

        // Clears progress bar, sets a login failed message, and shows login button
        private void OnLoginFailed()
        {
            _activityIndicator.Visibility = ViewStates.Invisible;
            _splachscreenMessage.Text = Resources.GetString(Resource.String.sign_in_error);
            _splachscreenMessage.Visibility = ViewStates.Visible;
            _signInButton.Alpha = 0f;
            _signInButton.Visibility = ViewStates.Visible;
            _signInButton.Animate().Alpha(1).SetDuration(1000).Start();
            _loginScreen.Invalidate();
        }

        // Clears progress bar, sets a permissions denied message, and shows login button
        private void OnAppPermissionsFailed()
        {
            OnLoginFailed();
            _splachscreenMessage.Text = Resources.GetString(Resource.String.app_permissions_error);
            _loginScreen.Invalidate();
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            ClearMessages();
            await ViewModel.Login();
        }

        // Called when returns from MSAL authentication
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }

        // Called when user responds to in app permissions request
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestLocationPermissionId:
                    {
                        var len = grantResults.Length;
                        if (len > 0 && grantResults[0] == Permission.Granted)
                        {
                            //Permission granted
                            ViewModel.InvokeLoginSucceed();
                            ClearMessages();
                            NavigateToMainViewModel();
                        }
                        else
                        {
                            //Permission Denied
                            var snack = Snackbar.Make(_loginScreen, Resources.GetString(Resource.String.app_permissions_error1), Snackbar.LengthShort);
                            snack.Show();
                            OnAppPermissionsFailed();
                        }
                    }
                    break;
            }
        }



        /*
                protected override void OnNewIntent(Intent intent)
                {
                    base.OnNewIntent(intent);

                    // uncomment  when start notification added
                    //var myextravalue = intent.GetStringExtra("somevalue");
                }

                public override void OnRequestPermissionsResult(int requestCode, string[] permissions, global::Android.Content.PM.Permission[] grantResults)
                {
                    PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                }
        */

    }
}