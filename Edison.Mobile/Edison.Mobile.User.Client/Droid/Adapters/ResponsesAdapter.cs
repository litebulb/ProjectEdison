using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Edison.Mobile.User.Client.Droid.Holders;
using Edison.Mobile.User.Client.Core.CollectionItemViewModels;
using Edison.Mobile.User.Client.Core.ViewModels;
using Android.Runtime;
using Android.Support.V7.CardView;
using Edison.Core.Common.Models;
using Android.Gms.Maps;
using Android.App;
using Edison.Mobile.Android.Common.Geolocation;
using Android.Gms.Maps.Model;

namespace Edison.Mobile.User.Client.Droid.Adapters
{
    public class ResponsesAdapter : RecyclerView.Adapter, IOnMapReadyCallback
    {
        private Context context;
        readonly ObservableRangeCollection<ResponseCollectionItemViewModel> _responses;
        private GoogleMap map;
        //public event EventHandler<int> OnResponseSelected;

        public ResponsesAdapter(Context context, ObservableRangeCollection<ResponseCollectionItemViewModel> responses)
        {
            this.context = context;
            this._responses = responses;
        }

        public override int ItemCount => _responses.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var response = _responses[position].Response;
            ResponseViewHolder responseViewHolder = (ResponseViewHolder)holder;
            responseViewHolder.Title.Text = response.ActionPlan.Name;
            responseViewHolder.Description.Text = response.ActionPlan.Description;
            responseViewHolder.Date.Text = response.StartDate.ToString();

            responseViewHolder.ColorBar.SetBackgroundColor(Constants.Colors.GetEventTypeColor((Activity)context, response.ActionPlan.Color));
            responseViewHolder.Image.SetImageResource(Constants.Assets.MapFromActionPlanIcon(response.ActionPlan.Icon));

            if (map != null)
            {
                var responseGeo = _responses[position].Geolocation;
                AddUserMarkerToMap(map, position);
                AddResponseMarkerToMap(map, position);

                var pos = new CameraPosition.Builder()
                    .Target(new LatLng(responseGeo.Latitude, responseGeo.Longitude))
                    .Zoom(16)
                    .Build();

                map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(pos));
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View responseCardView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.response_card, parent, false);
            MapView mapView = (MapView)responseCardView.FindViewById(Resource.Id.response_mapview);
           
            if (mapView != null)
            {
                mapView.OnCreate(null);
                mapView.GetMapAsync(this);
            }

            return new ResponseViewHolder(responseCardView);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            map.UiSettings.MapToolbarEnabled = false;
            map.UiSettings.ZoomControlsEnabled = false;
            MapsInitializer.Initialize(context);
        }

        public void AddUserMarkerToMap(GoogleMap map, int position)
        {
            var locationService = new LocationService();
            var userGeo = locationService.LastKnownLocation;
            var icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.user);
            var pos = new LatLng(userGeo.Latitude, userGeo.Longitude);

            var markerOptions = new MarkerOptions()
                .SetPosition(pos)
                .SetIcon(icon);

            map.AddMarker(markerOptions);
        }

        public void AddResponseMarkerToMap(GoogleMap map, int position)
        {
            var responseGeo = _responses[position].Geolocation;
            var pos = new LatLng(responseGeo.Latitude, responseGeo.Longitude);
            var icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.pin);

            var markerOptions = new MarkerOptions()
                .SetPosition(pos)
                .SetIcon(icon);

            map.AddMarker(markerOptions);
        }
    }

}
