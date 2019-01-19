using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Android.Content;
using Android.Views;
using Android.Content.Res;
using Android.Support.V7.Widget;

using Edison.Mobile.User.Client.Droid.Holders;
using Edison.Mobile.User.Client.Core.CollectionItemViewModels;


namespace Edison.Mobile.User.Client.Droid.Adapters
{
    public class ResponsesAdapter : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;

        private Context _context;



        public ObservableRangeCollection<ResponseCollectionItemViewModel> Responses { get; private set; } = new ObservableRangeCollection<ResponseCollectionItemViewModel>();

        public override int ItemCount => Responses.Count;


        public ResponsesAdapter(Context ctx, ObservableRangeCollection<ResponseCollectionItemViewModel> responses)
        {
            _context = ctx;
            Responses = responses;
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
            // Add margin to act as seperaor betwen itmes. Double size for first item to center the first card
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

            // if the details have not been fetched (the inital API only fetches a summary), fetch it
            if (response?.ActionPlan == null)
            {
                vh.Loading.Visibility = ViewStates.Visible;
                UpdateResponseDetails(position);
            }
            if (response != null)
            {
                vh.Loading.Visibility = ViewStates.Gone;
                // Set the contents in this ViewHolder's CardView  from this position in the collection:
                var color = Constants.GetEventTypeColor(_context, response.Color);
                vh.Seperator.SetBackgroundColor(color);
                vh.Icon.BackgroundTintList = ColorStateList.ValueOf(color);
                vh.Icon.SetImageResource(GetIconResourceId(response.Icon));
                vh.Alert.Text = response.Name;
                vh.AlertTime.Text = response.StartDate.ToString();
                vh.AlertDescription.Text = response.ActionPlan?.Description;
                if (response.Geolocation != null)
                    vh.SetMapLocation(response.Geolocation.Latitude, response.Geolocation.Longitude, color);
            }
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

    }

}