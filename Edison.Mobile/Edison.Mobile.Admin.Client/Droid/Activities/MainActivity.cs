using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Widget;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Android.Common.Ioc;
using Edison.Mobile.Common.Ioc;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Identity.Client;
using System;

namespace Edison.Mobile.Admin.Client.Droid.Activities
{
    [Activity(Label = "", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : BaseActivity<MainViewModel>
    {
        private LinearLayout _setupNewButton;
        private LinearLayout _manageButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.Main);

            BindResources();
            BindVMEvents();
        }

        private void BindResources()
        {
            _setupNewButton = FindViewById<LinearLayout>(Resource.Id.button_new);
            _manageButton = FindViewById<LinearLayout>(Resource.Id.button_manage);
        }

        protected void BindVMEvents()
        {
            _setupNewButton.Click += OnSetupNewButtonClick;
            _manageButton.Click += OnManageButtonClick;
        }

        private async void OnManageButtonClick(object sender, EventArgs e)
        {
            //ClearMessages();

        }

        private async void OnSetupNewButtonClick(object sender, EventArgs e)
        {
            //ClearMessages();
            
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
    }
}

