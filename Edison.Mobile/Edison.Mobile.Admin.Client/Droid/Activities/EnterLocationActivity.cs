using System.Linq;
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
using System;
using System.Collections.Generic;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using TextTypes = Android.Text.InputTypes;
using Android.Gms.Maps;
using Android.Support.Fragment;
using Android.Gms.Maps.Model;
using System.Threading.Tasks;
using Java.Lang;
using Edison.Mobile.Admin.Client.Droid.Toolbars;
using Android.Runtime;

namespace Edison.Mobile.Admin.Client.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/EdisonLight.Base", WindowSoftInputMode = SoftInput.AdjustResize, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, Icon = "@mipmap/ic_edison_launcher")]
    public class EnterLocationActivity : BaseActivity<ManageDeviceViewModel>, IOnMapReadyCallback
    {
        MapFragment mapFragment;
        GoogleMap googleMap;
        private string _latitude;
        private string _longitude;
        private LatLng _location;

        public async void OnMapReady(GoogleMap map)
        {
            googleMap = map;

            googleMap.UiSettings.ZoomControlsEnabled = true;           
            googleMap.UiSettings.MyLocationButtonEnabled = true;

            var gpsLocation = await ViewModel.GetLastKnownLocation();
            _location = new LatLng(gpsLocation.Latitude, gpsLocation.Longitude);

            googleMap.MapClick += GoogleMap_MapClick;

            AddMarker();
        }

        private void AddMarker()
        {
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(_location);
            builder.Zoom(16);

            CameraPosition cameraPosition = builder.Build();

            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            googleMap.MoveCamera(cameraUpdate);

            var marker = new MarkerOptions()
            .SetPosition(_location)
            .SetTitle("Current");

            googleMap.AddMarker(marker);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (data.HasExtra("latitude") && data.HasExtra("longitude"))
            {
                _latitude = data.GetStringExtra("latitude");
                _longitude = data.GetStringExtra("longitude");

                if (!string.IsNullOrWhiteSpace(_latitude) && !string.IsNullOrWhiteSpace(_longitude))
                {
                    _location = new LatLng(double.Parse(_latitude), double.Parse(_longitude));                    

                    AddMarker();
                }
            }
        }

        private async void GoogleMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            var intent = new Intent(this, typeof(EnterLocationFullscreenActivity));
            intent.AddFlags(ActivityFlags.NoAnimation);            
            StartActivityForResult(intent, 1);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
                       
            SetContentView(Resource.Layout.enter_location);

            BindResources();

            //BindVMEvents();
        }

        private void BindResources()
        {            
            var toolbar = FindViewById<CenteredToolbar>(Resource.Id.toolbar);
            toolbar.SetTitle(Resource.String.edison_device_setup_message);

            var layout = FindViewById<LinearLayout>(Resource.Id.instruction);


            var instructionNumber = layout.FindViewById<AppCompatTextView>(Resource.Id.instruction_number);
            var instructionText = layout.FindViewById<AppCompatTextView>(Resource.Id.instruction_text);


            instructionNumber.Text = "6";
            instructionText.SetText(Resource.String.device_details_instruction_label);

            SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                        
            this.BackPressed += SelectWifiOnDeviceActivity_BackPressed;
            
            mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            var button = FindViewById<AppCompatButton>(Resource.Id.complete_setup_button);
            button.Click += Button_Click;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Operation confirmation");
            builder.SetMessage("Continue with command?");
            builder.SetPositiveButton("Yes", (s, args) => { /* do stuff on OK */ });
            builder.SetNegativeButton("No", (s, args) => { });
            builder.SetCancelable(false);
            builder.Show();
        }

        private void SelectWifiOnDeviceActivity_BackPressed(object sender, EventArgs e)
        {
            Finish();
        }

        
    }
}

