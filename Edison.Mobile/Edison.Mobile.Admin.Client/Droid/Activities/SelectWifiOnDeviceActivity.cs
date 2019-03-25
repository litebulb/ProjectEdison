using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.Models;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Admin.Client.Droid.Adapters;
using Edison.Mobile.Admin.Client.Droid.Toolbars;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Android.Common.Ioc;
using Edison.Mobile.Common.Ioc;
using Java.Lang;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Edison.Mobile.Admin.Client.Droid.Activities
{
    [Activity(Label = "@string/app_name", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, Icon = "@mipmap/ic_edison_launcher")]
    public class SelectWifiOnDeviceActivity : BaseActivity<SelectWifiViewModel>
    {
        private RecyclerView _recyclerView;
        private RecyclerView.LayoutManager _layoutManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.select_wifi_on_device);
            
            BindResources();
            BindVMEvents();
        }

        private void BindResources()
        {            
            var toolbar = FindViewById<CenteredToolbar>(Resource.Id.toolbar);
            toolbar.SetTitle(Resource.String.edison_device_setup_message);

            var layout = FindViewById<LinearLayout>(Resource.Id.instruction);

            var instructionNumber = layout.FindViewById<AppCompatTextView>(Resource.Id.instruction_number);
            var instructionText = layout.FindViewById<AppCompatTextView>(Resource.Id.instruction_text);

            instructionNumber.Text = "4";
            instructionText.SetText(Resource.String.select_wifi_network_label);
            
            SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                        
            _layoutManager = new LinearLayoutManager(this);

            // Get our RecyclerView layout:
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.selectWifiRecyclerView);
            _recyclerView.SetLayoutManager(_layoutManager);

            this.BackPressed += SelectWifiOnDeviceActivity_BackPressed; 

        }

        private void SelectWifiOnDeviceActivity_BackPressed(object sender, EventArgs e)
        {
            Finish();
        }

        protected void BindVMEvents()
        {
            ViewModel.OnAvailableWifiNetworksChanged += ViewModel_OnAvailableWifiNetworksChanged;
        }

        private void ViewModel_OnAvailableWifiNetworksChanged()
        {
            RunOnUiThread(new Runnable(() =>
            {
                var devices = new List<AvailableNetwork>(ViewModel.AvailableWifiNetworks);

                // Instantiate the adapter and pass in its data source:
                var mAdapter = new WifiSelectionDeviceAdapter(devices, GoToEnterPassword);

                // Plug the adapter into the RecyclerView:
                _recyclerView.SetAdapter(mAdapter);
                
            }));
        }

        public void GoToEnterPassword(string networkSsid)
        {
            this.ViewModel.SetWifi(networkSsid);

            var intent = new Intent(this, typeof(EnterPasswordActivity));
            intent.AddFlags(ActivityFlags.NoAnimation);
            intent.PutExtra("ssid", networkSsid);            
            StartActivity(intent);
        }
        
    }
}

