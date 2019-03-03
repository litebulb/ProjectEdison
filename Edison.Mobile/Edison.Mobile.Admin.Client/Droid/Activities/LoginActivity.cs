using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Android.Common;
using Microsoft.Identity.Client;

namespace Edison.Mobile.Admin.Client.Droid.Activities
{
    [Activity(Theme ="@style/EdisonLight.Fullscreen", MainLauncher = true, Icon = "@mipmap/ic_edison_launcher", Exported = true, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginActivity : BaseActivity<LoginViewModel>
    {
        const int RequestCameraPermisionID = 1001;
        const int RequestWifiStateID = 1002;
        const int RequestLocationID = 1003;

        private AppCompatButton _signInButton;

        private bool cameraPermissionAllowed = false;
        private bool wifiPermissionAllowed = false;
        private bool locationPermissionAllowed = false;
        private bool readyToLogin = false;

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (grantResults.Length > 0)
            {
                switch (requestCode)
                {
                    case RequestCameraPermisionID:
                        {
                            if (grantResults[0] == Permission.Granted)
                            {
                                cameraPermissionAllowed = true;
                            }
                        }
                        break;
                    case RequestWifiStateID:
                        {
                            if (grantResults[0] == Permission.Granted)
                            {
                                wifiPermissionAllowed = true;
                            }
                        }
                        break;
                    case RequestLocationID:
                        {
                            if (grantResults[0] == Permission.Granted)
                            {
                                locationPermissionAllowed = true;
                            }
                        }
                        break;
                }
                ValidatePermissions();
            }

        }

        private void ValidatePermissions()
        {
            if (!cameraPermissionAllowed) { RequestCameraPermissions(); }
            if (!wifiPermissionAllowed) { RequestWifiPermissions(); }
            if (!locationPermissionAllowed) { RequestLocationPermissions(); }

            _signInButton.Enabled = (cameraPermissionAllowed && wifiPermissionAllowed && locationPermissionAllowed);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (ViewModel != null)
                ((LoginViewModel)ViewModel).AuthService.UiParent = new UIParent(this);

            SetContentView(Resource.Layout.screen_login);
            BindResources();
            BindVMEvents();

            RequestPermissions();

        }

        private void RequestPermissions()
        {
            ValidatePermissions();
        }

        private void RequestLocationPermissions()
        {
            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.AccessFineLocation) != Permission.Granted)
            {
                //Request Permision  
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessFineLocation }, RequestLocationID);
                return;
            }
            else
            {
                locationPermissionAllowed = true;
            }
        }

        private void RequestWifiPermissions()
        {
            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.ChangeWifiState) != Permission.Granted)
            {
                //Request Permision  
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ChangeWifiState }, RequestWifiStateID);
                return;
            }
            else
            {
                wifiPermissionAllowed = true;
            }
        }

        private void RequestCameraPermissions()
        {
            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Permission.Granted)
            {
                //Request Permision  
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.Camera }, RequestCameraPermisionID);
                return;
            }
            else
            {
                cameraPermissionAllowed = true;
            }
        }

        private void BindResources()
        {
            _signInButton = FindViewById<AppCompatButton>(Resource.Id.sign_in_button);
            _signInButton.Enabled = false;
        }

        // Called when returns from MSAL authentication
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }


        protected void BindVMEvents()
        {
            ViewModel.OnDisplayLogin += SignIn;
            //ViewModel.ClearLoginMessages += ClearMessages;
            ViewModel.OnLoginFailed += OnLoginFailed;
            ViewModel.OnNavigateToMainViewModel += NavigateToMainViewModel;
            _signInButton.Click += OnButtonClick;
        }

        protected void UnBindVMEvents()
        {            
            ViewModel.OnDisplayLogin -= SignIn;
            //ViewModel.ClearLoginMessages -= ClearMessages;
            ViewModel.OnLoginFailed -= OnLoginFailed;
            ViewModel.OnNavigateToMainViewModel -= NavigateToMainViewModel;
            _signInButton.Click -= OnButtonClick;
        }

        private void OnLoginFailed()
        {
            //_activityIndicator.Visibility = ViewStates.Invisible;
            //_splachscreenMessage.Text = Resources.GetString(Resource.String.sign_in_error);
            //_splachscreenMessage.Visibility = ViewStates.Visible;
            //_signInButton.Alpha = 0f;
            //_signInButton.Visibility = ViewStates.Visible;
            //_signInButton.Animate().Alpha(1).SetDuration(1000).Start();
            //_loginScreen.Invalidate();
        }

        private void NavigateToMainViewModel()
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.NoAnimation);
            StartActivity(intent);
            Finish();
        }

        private async void OnButtonClick(object sender, EventArgs e)
        {
            //ClearMessages();
            await ViewModel.Login();
        }

        private async void SignIn()
        {
            if (cameraPermissionAllowed && wifiPermissionAllowed && locationPermissionAllowed)
            {
                await ViewModel.Login();
            }
            else
            {
                readyToLogin = true;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnBindVMEvents();
        }
    }
}