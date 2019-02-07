using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Android.Common;
using Microsoft.Identity.Client;

namespace Edison.Mobile.Admin.Client.Droid.Activities
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@mipmap/ic_edison_launcher", Exported = true, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginActivity : BaseActivity<LoginViewModel>
    {

        private AppCompatButton _signInButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (ViewModel != null)
                ((LoginViewModel)ViewModel).AuthService.UiParent = new UIParent(this);

            SetContentView(Resource.Layout.screen_login);
            BindResources();
            BindVMEvents();

        }
        private void BindResources()
        {
            _signInButton = FindViewById<AppCompatButton>(Resource.Id.sign_in_button);         
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
            await ViewModel.Login();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnBindVMEvents();
        }
    }
}