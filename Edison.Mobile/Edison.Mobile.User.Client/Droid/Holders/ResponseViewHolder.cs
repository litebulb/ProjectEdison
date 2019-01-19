using System;

using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Support.V4.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Edison.Mobile.User.Client.Droid.Holders
{
    public class ResponseViewHolder : RecyclerView.ViewHolder, IOnMapReadyCallback
    {
        private View _layout;

        private float _colorHue = -1;

        // Declare Content views
        public CardView Card { get; private set; }
        public View Ripple { get; private set; }
        public ProgressBar Loading { get; private set; }
        public MapView Map { get; private set; }
        public GoogleMap GMap { get; private set; }
        public View Seperator { get; private set; }
        public AppCompatImageView Icon { get; private set; }
        public AppCompatTextView Alert { get; private set; }
        public AppCompatTextView AlertTime { get; private set; }
        public AppCompatTextView AlertDescription { get; private set; }
        public AppCompatTextView Button { get; private set; }

        public ResponseViewHolder(View item, Action<int> listener) : base(item)
        {
            _layout = item;
            BindViews(item);
            //          item.Click += (s,e) => listener(base.LayoutPosition);
            Ripple.Click += (s, e) => listener(base.LayoutPosition);
            //         Button.Click += (s,e) => listener(base.LayoutPosition);  // dont really need this!
            InitializeMapView();
        }


        private void BindViews(View item)
        {
            Card = item.FindViewById<CardView>(Resource.Id.card);
            Ripple = item.FindViewById<View>(Resource.Id.ripple);
            Loading = item.FindViewById<ProgressBar>(Resource.Id.card_loading);
            Map = item.FindViewById<MapView>(Resource.Id.card_map);
            Seperator = item.FindViewById<View>(Resource.Id.card_seperator);
            Icon = item.FindViewById<AppCompatImageView>(Resource.Id.card_icon);
            Alert = item.FindViewById<AppCompatTextView>(Resource.Id.card_alert);
            AlertTime = item.FindViewById<AppCompatTextView>(Resource.Id.card_alert_time);
            AlertDescription = item.FindViewById<AppCompatTextView>(Resource.Id.card_alert_description);
            Button = item.FindViewById<AppCompatTextView>(Resource.Id.more_info_btn);
        }

        private void InitializeMapView()
        {
            Map?.OnCreate(null);
            Map?.GetMapAsync(this);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            MapsInitializer.Initialize(Application.Context);
            GMap = googleMap;
            GMap.UiSettings.CompassEnabled = false;
            GMap.UiSettings.MyLocationButtonEnabled = false;
            GMap.UiSettings.MapToolbarEnabled = false;
            SetMapLocation();
        }

        private void SetMapLocation()
        {
            if (GMap == null) return;

            var location = Map.Tag as LatLng;
            if (location == null) return;

            // Add a marker for this item and set the camera
            var cameraUpdate = CameraUpdateFactory.NewLatLng(location);
            GMap.MoveCamera(cameraUpdate);

            var markerOptions = new MarkerOptions();
            markerOptions.SetPosition(location);
            if (_colorHue > -1)
            {
                var bmDescriptor = BitmapDescriptorFactory.DefaultMarker(_colorHue);
                markerOptions.SetIcon(bmDescriptor);
            }

            GMap.AddMarker(markerOptions);



            // Set the map type back to normal.
            GMap.MapType = GoogleMap.MapTypeNormal;
        }


        public void SetMapLocation(double latitude, double longitude, Color eventcolor)
        {
            var location = new LatLng(latitude, longitude);
            _layout.Tag = this;
            Map.Tag = location;
            float[] hsl = new float[3];
            ColorUtils.ColorToHSL(eventcolor, hsl);
            _colorHue = hsl[0];
            SetMapLocation();
        }



    }
}