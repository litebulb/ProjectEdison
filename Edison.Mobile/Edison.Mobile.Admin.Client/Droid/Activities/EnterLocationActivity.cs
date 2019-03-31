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
using Edison.Mobile.Admin.Client.Droid.Dialogs;
using Android.Graphics.Drawables;
using Color = Android.Graphics.Color;

namespace Edison.Mobile.Admin.Client.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/EdisonLight.Base", WindowSoftInputMode =  SoftInput.StateHidden|SoftInput.AdjustResize, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, Icon = "@mipmap/ic_edison_launcher")]
    public class EnterLocationActivity : BaseActivity<ManageDeviceViewModel>, IOnMapReadyCallback
    {
        public const string Latitude = "latitude";
        public const string Longitude = "longitude";

        MapFragment mapFragment;
        GoogleMap googleMap;
        private LatLng _location;

        private AppCompatEditText nameEditText;
        private AppCompatEditText buildingEditText;
        private AppCompatEditText floorEditText;
        private AppCompatEditText roomEditText;
        private AppCompatTextView wifiTextView;

        public async void OnMapReady(GoogleMap map)
        {
            googleMap = map;

            googleMap.UiSettings.ZoomControlsEnabled = true;           
            googleMap.UiSettings.MyLocationButtonEnabled = true;

            if (_location == default(LatLng))
            {
                var gpsLocation = await ViewModel.GetLastKnownLocation();
                _location = new LatLng(gpsLocation.Latitude, gpsLocation.Longitude);
            }

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

            if (data.HasExtra(Latitude) && data.HasExtra(Longitude))
            {
                var latitude = data.GetDoubleExtra(Latitude, default(double));
                var longitude = data.GetDoubleExtra(Longitude, default(double));

                if (latitude != default(double) && longitude != default(double))
                {
                    _location = new LatLng(latitude, longitude);          
                    AddMarker();
                }
            }
        }

        private async void GoogleMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            var intent = new Intent(this, typeof(EnterLocationFullscreenActivity));
            intent.AddFlags(ActivityFlags.NoAnimation);
            intent.PutExtra(Latitude, _location.Latitude);
            intent.PutExtra(Longitude, _location.Longitude);
            StartActivityForResult(intent, 1);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
                       
            SetContentView(Resource.Layout.enter_location);
            
            Task.Run(async () => await BindResources());            
        }

        private async Task BindResources()
        {
            this.ViewModel.CheckingConnectionStatusUpdated += ViewModel_CheckingConnectionStatusUpdated;
            this.ViewModel.OnDeviceUpdated += ViewModel_OnDeviceUpdated;
            RunOnUiThread(() =>
            {

                nameEditText = FindViewById<AppCompatEditText>(Resource.Id.nameEditText);
                buildingEditText = FindViewById<AppCompatEditText>(Resource.Id.buildingEditText);
                floorEditText = FindViewById<AppCompatEditText>(Resource.Id.floorEditText);
                roomEditText = FindViewById<AppCompatEditText>(Resource.Id.roomEditText);
                wifiTextView = FindViewById<AppCompatTextView>(Resource.Id.wifiTextView);

                ReconcileEditText(i => i.Name, nameEditText);
                ReconcileEditText(i => i.Location1, buildingEditText);
                ReconcileEditText(i => i.Location2, floorEditText);
                ReconcileEditText(i => i.Location3, roomEditText);

                if (this.ViewModel.CurrentDeviceModel != null && this.ViewModel.CurrentDeviceModel.Geolocation != null)
                {
                    _location = new LatLng(this.ViewModel.CurrentDeviceModel.Geolocation.Latitude, this.ViewModel.CurrentDeviceModel.Geolocation.Longitude);
                }

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

                var wifiLayout = FindViewById<LinearLayout>(Resource.Id.wifiLayout);
                wifiLayout.Click += WifiLayout_Click;
            });
            
            await this.ViewModel.GetDeviceNetworkInfo();
        }

        private void ViewModel_OnDeviceUpdated(object sender, bool e)
        {
            if (!e)
            {
                Dialog dialog = new Dialog(this);
                dialog.SetTitle("Error occured");                
                dialog.Show();
            }

            SetupCompleteDialog setupCompleteDialog = new SetupCompleteDialog(this);
            setupCompleteDialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            setupCompleteDialog.Show();

            setupCompleteDialog.GoToHome += (s, eh) =>
            {
                Intent home = new Intent(this, typeof(MainActivity));
                home.SetFlags(ActivityFlags.NewTask);
                StartActivity(home);
            };

            setupCompleteDialog.GoToManageDevices += (s, eh) =>
            {
                setupCompleteDialog.Cancel();
            };

        }

        private void ViewModel_CheckingConnectionStatusUpdated(object sender, CheckingConnectionStatusUpdatedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                wifiTextView.Text = e.StatusText;
            });
        }

        private void WifiLayout_Click(object sender, EventArgs e)
        {
            Intent home = new Intent(this, typeof(SelectWifiOnDeviceActivity));
            home.SetFlags(ActivityFlags.NewTask);
            StartActivity(home);
        }

        private async void Button_Click(object sender, EventArgs e)
        {
            this.ViewModel.CurrentDeviceModel.Name = ValidateInput(nameEditText.Text);            
            this.ViewModel.CurrentDeviceModel.Location1 = ValidateInput(buildingEditText.Text);
            this.ViewModel.CurrentDeviceModel.Location2 = ValidateInput(floorEditText.Text);
            this.ViewModel.CurrentDeviceModel.Location3 = ValidateInput(roomEditText.Text);
            this.ViewModel.CurrentDeviceModel.Geolocation = new Geolocation() { Latitude = _location.Latitude, Longitude = _location.Longitude };
            this.ViewModel.CurrentDeviceModel.Enabled = true;            

            await this.ViewModel.UpdateDevice();
        }

        private string ValidateInput(string value)
        {
            if(value == GetString(Resource.String.edit_text_label))
            {
                return default(string);
            }

            return value;
        }

        private void ReconcileEditText(Func<DeviceModel, string> prop, AppCompatEditText editText)
        {
            string value = prop(this.ViewModel.CurrentDeviceModel);

            if (value != default(string))
            {            
                editText.Text = value;
            }
        }

        private void SelectWifiOnDeviceActivity_BackPressed(object sender, EventArgs e)
        {
            Finish();
        }

        
    }
}

