using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.Droid.Adapters;
using Edison.Mobile.Admin.Client.Droid.Toolbars;
using Edison.Mobile.Android.Common;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;

namespace Edison.Mobile.Admin.Client.Droid.Activities
{
    [Activity(ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/EdisonLight.Base", Icon = "@mipmap/ic_edison_launcher")]
    public class ManageDevicesActivity : BaseActivity<ManageDeviceViewModel>
    {

        private RecyclerView _recyclerView;
        private RecyclerView.LayoutManager _layoutManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.manage_devices);

            BindResources();
            BindVMEvents();
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }
        
        private void BindResources()
        {            
            var toolbar = FindViewById<CenteredToolbar>(Resource.Id.toolbar);
            toolbar.SetTitle(Resource.String.edison_device_setup_message);
            
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            _layoutManager = new LinearLayoutManager(this);

            // Get our RecyclerView layout:
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
             _recyclerView.SetLayoutManager(_layoutManager);

            this.BackPressed += (s, e) => { Finish(); };
        }

        protected void BindVMEvents()
        {
            ViewModel.NearDevices.CollectionChanged += NearDevices_CollectionChanged;;
        }


        public void NearDevices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var devices = new List<DeviceModel>(ViewModel.NearDevices);

            // Instantiate the adapter and pass in its data source:
            var mAdapter = new NearbyDeviceAdapter(devices, GoToManageDevice);            

            // Plug the adapter into the RecyclerView:
            _recyclerView.SetAdapter(mAdapter);
        }

        public async void GoToManageDevice(Guid deviceId)
        {
            this.ViewModel.SetDeviceModel(ViewModel.NearDevices.First(i => i.DeviceId == deviceId));
            await this.ViewModel.SetKeys();

            var intent = new Intent(this, typeof(EnterLocationActivity));
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

