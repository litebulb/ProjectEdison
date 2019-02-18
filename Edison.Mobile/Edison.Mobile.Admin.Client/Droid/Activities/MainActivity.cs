using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.Droid.Adapters;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Android.Common.Ioc;
using Edison.Mobile.Common.Ioc;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Edison.Mobile.Admin.Client.Droid.Activities
{
    [Activity(Label = "test", MainLauncher = true, Theme = "@style/EdisonLight.TransparentBar", Icon = "@mipmap/icon")]
    public class MainActivity : BaseActivity<MainViewModel>
    {
        private LinearLayout _setupNewButton;
        private LinearLayout _manageButton;

        private RecyclerView _recyclerView;
        private RecyclerView.LayoutManager _layoutManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.Main);

            BindResources();
            BindVMEvents();
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }

        private void BindResources()
        {
            _setupNewButton = FindViewById<LinearLayout>(Resource.Id.button_new);
            _manageButton = FindViewById<LinearLayout>(Resource.Id.button_manage);
            
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            TextView mTitle = (TextView)toolbar.FindViewById(Resource.Id.toolbar_title);
            mTitle.Text = GetString(Resource.String.edison_device_setup_message);

            SetSupportActionBar(toolbar);            
            
            SupportActionBar.SetIcon(Resource.Drawable.menu);

            _layoutManager = new LinearLayoutManager(this);

            // Get our RecyclerView layout:
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(_layoutManager);

        }

        protected void BindVMEvents()
        {
            _setupNewButton.Click += OnSetupNewButtonClick;
            _manageButton.Click += OnManageButtonClick;
            
            ViewModel.NearDevices.CollectionChanged += NearDevices_CollectionChanged;;
        }

        public void NearDevices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var devices = new List<DeviceModel>(ViewModel.NearDevices);

            // Instantiate the adapter and pass in its data source:
            var mAdapter = new NearbyDeviceAdapter(devices);            

            // Plug the adapter into the RecyclerView:
            _recyclerView.SetAdapter(mAdapter);
        }


        private async void OnManageButtonClick(object sender, EventArgs e)
        {
            //ClearMessages();

        }

        private async void OnSetupNewButtonClick(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(NewDeviceSetupActivity));
            intent.AddFlags(ActivityFlags.NoAnimation);
            StartActivity(intent);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
    }
}

