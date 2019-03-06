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

namespace Edison.Mobile.Admin.Client.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/EdisonLight.Base", WindowSoftInputMode = SoftInput.AdjustResize, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, Icon = "@mipmap/ic_edison_launcher")]
    public class EnterLocationFullscreenActivity : BaseActivity<ManageDeviceViewModel>, IOnMapReadyCallback
    {
        public const string Latitude = "latitude";
        public const string Longitude = "longitude";

        private MapFragment mapFragment;
        private GoogleMap googleMap;
        private Marker _marker;
        private LatLng _location;

        public async void OnMapReady(GoogleMap map)
        {
            googleMap = map;

            googleMap.UiSettings.ZoomControlsEnabled = true;
           // googleMap.MyLocationEnabled = true;
            googleMap.UiSettings.MyLocationButtonEnabled = true;
            
            AddMarker();

            googleMap.MarkerDragEnd += GoogleMap_MarkerDragEnd;
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
            .Draggable(true)
            .SetTitle("Current");

            _marker = googleMap.AddMarker(marker);
        }


        private void GoogleMap_MarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e)
        {
            _marker.Position = e.Marker.Position;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
                       
            SetContentView(Resource.Layout.enter_location_fullscreen);

            var latitude = this.Intent.GetDoubleExtra(Latitude, default(double));
            var longitude = this.Intent.GetDoubleExtra(Longitude, default(double));

            if (latitude != default(double) && longitude != default(double))
            {
                _location = new LatLng(latitude, longitude);
            }

            BindResources();
        }
        
        private void BindResources()
        {            
            var toolbar = FindViewById<CenteredToolbar>(Resource.Id.toolbar);
            toolbar.SetTitle(Resource.String.edison_device_setup_message);
            
            SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                        
            this.BackPressed += SelectWifiOnDeviceActivity_BackPressed;
            
            mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            var button = FindViewById<AppCompatButton>(Resource.Id.done_button);
            button.Click += Button_Click;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent();            
            intent.PutExtra(Latitude, _marker.Position.Latitude);
            intent.PutExtra(Longitude, _marker.Position.Longitude);
            SetResult(Result.Ok, intent);
            Finish();
      
        }

        private void SelectWifiOnDeviceActivity_BackPressed(object sender, EventArgs e)
        {
            Finish();
        }

    }
}

