using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Android.Content;
using Android.Views;
using Android.Content.Res;
using Android.Support.V7.Widget;

using Edison.Mobile.User.Client.Droid.Holders;
using Edison.Mobile.User.Client.Core.CollectionItemViewModels;
using Android.Gms.Maps.Model;
using Edison.Mobile.Common.Geo;
using Android.Graphics;

namespace Edison.Mobile.User.Client.Droid.Adapters
{
    public class ResponsesAdapter : RecyclerView.Adapter
    {

        public event EventHandler<LocationChangedEventArgs> LocationChanged;
        public event EventHandler<int> ItemClick;

        private Context _context;


        private EdisonLocation _oldUserLocation = null;
        private LatLng _userLocation = null;
        public LatLng UserLocation
        {
            get => _userLocation;
            set
            {
                _userLocation = value;
                LocationChanged?.Invoke(null, new LocationChangedEventArgs(_oldUserLocation, new EdisonLocation(_userLocation.Latitude, _userLocation.Longitude)));
   //             UpdateMap();
            }
        }

        public ObservableRangeCollection<ResponseCollectionItemViewModel> Responses { get; private set; } = new ObservableRangeCollection<ResponseCollectionItemViewModel>();

        public override int ItemCount => Responses.Count;


        public ResponsesAdapter(Context ctx, ObservableRangeCollection<ResponseCollectionItemViewModel> responses, EdisonLocation userLocation)
        {
            _context = ctx;
            Responses = responses;
            if (userLocation != null)
                UserLocation = new LatLng(userLocation.Latitude, userLocation.Longitude);
        }



        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView
            View responseCard = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.response_card, parent, false);
            // Create a ViewHolder to find and hold these view references, and register OnClick with the view holder:
            ResponseViewHolder vh = new ResponseViewHolder(responseCard, OnClick);
            return vh;
        }


        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ResponseViewHolder vh = holder as ResponseViewHolder;
            // Add margin to act as seperator between itmes. Double size for first item to center the first card
            ViewGroup.MarginLayoutParams lp = (ViewGroup.MarginLayoutParams)vh.Card.LayoutParameters;
            lp.Width = Constants.EventResponseCardWidthPx;
            if (position == 0)
                lp.LeftMargin = Constants.EventResponseCardSeperatorWidthPx;
            else
                lp.LeftMargin = Constants.EventResponseCardSeperatorWidthPx / 2;
            if (position == Responses.Count - 1)
                lp.RightMargin = Constants.EventResponseCardSeperatorWidthPx;
            vh.Card.LayoutParameters = lp;

            // Try to get the actual response details
            var response = Responses[position].Response;

            if (response == null) return;

            if (response?.ActionPlan == null)
                // The details have not been fetched (the inital API only fetches a summary), fetch it
                UpdateResponseDetails(position);

            vh.Loading.Visibility = response.ActionPlan == null ? ViewStates.Visible : ViewStates.Gone;

            var eventColorName = string.IsNullOrWhiteSpace(response.ActionPlan?.Color) ? response.Color : response.ActionPlan.Color;
            var eventColor = Constants.GetEventTypeColor(_context, eventColorName);
            vh.Seperator.SetBackgroundColor(eventColor);
            var eventIconName = string.IsNullOrWhiteSpace(response.ActionPlan?.Icon) ? response.Icon : response.ActionPlan.Icon;
            vh.Icon.SetImageResource(GetIconResourceId(eventIconName));
            vh.Icon.BackgroundTintList = ColorStateList.ValueOf(eventColor);
            vh.Alert.Text = string.IsNullOrWhiteSpace(response.ActionPlan?.Name) ? response.Name : response.ActionPlan.Name;
            vh.AlertDescription.Text = string.IsNullOrWhiteSpace(response.ActionPlan?.Description) ? null : response.ActionPlan.Description;
            vh.AlertTime.Text = response.StartDate.ToString();

            LatLng eventLocation = null;
            //_userLocation = new LatLng(41.885796, -87.624911);  // For testing
            //eventLatitude = new LatLng(41.883408, -87.621907);  // For testing
            var eventGeolocation = response.Geolocation == null ? Responses[position].Geolocation : response.Geolocation;
            if (eventGeolocation != null)
                eventLocation = new LatLng(eventGeolocation.Latitude, eventGeolocation.Longitude);
            if (eventLocation != null || UserLocation != null)
                vh.SetupMap(eventColor, UserLocation, eventLocation);

            LocationChanged -= vh.OnLocationChanged;
            LocationChanged += vh.OnLocationChanged;

        }







        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }


        private void UpdateResponseDetails(int position)
        {
            Task.Run(async () =>
            {
                await GetResponseDetailsAsync(position);
            });
        }

        private async Task GetResponseDetailsAsync(int position)
        {
            await Responses[position].GetResponse();
            NotifyItemChanged(position);
        }



        private int GetIconResourceId(string iconName)
        {
            var id = _context.Resources.GetIdentifier(iconName, "drawable", _context.PackageName);
            return id == 0 ? Resource.Drawable.emergency : id;
        }



        public void OnLocationChanged(object s, LocationChangedEventArgs e)
        {
            _oldUserLocation = e.LastLocation;
            UserLocation = new LatLng(e.CurrentLocation.Latitude, e.CurrentLocation.Longitude);
        }


    }

}