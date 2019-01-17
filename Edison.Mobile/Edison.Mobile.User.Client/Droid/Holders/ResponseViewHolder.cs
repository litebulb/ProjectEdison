using System;
using Android.Content;
using Android.Gms.Maps;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Edison.Mobile.User.Client.Droid.Holders
{
    public class ResponseViewHolder : RecyclerView.ViewHolder
    {
        public MapView mapView { get; set; }
        public View ColorBar { get; set; }
        public ImageView Image { get; set; }
        public TextView Title { get; set; }
        public TextView Date { get; set; }
        public TextView Description { get; set; }
        public Button InfoButton { get; set; }

        public ResponseViewHolder(View responseCard) : base(responseCard)
        {
            mapView = (MapView)responseCard.FindViewById(Resource.Id.response_mapview);
            ColorBar = responseCard.FindViewById<View>(Resource.Id.card_color_bar);
            Image = responseCard.FindViewById<ImageView>(Resource.Id.card_icon);
            Title = responseCard.FindViewById<TextView>(Resource.Id.card_alert);
            Date = responseCard.FindViewById<TextView>(Resource.Id.card_alert_time);
            Description = responseCard.FindViewById<TextView>(Resource.Id.card_alert_description);
            InfoButton = responseCard.FindViewById<Button>(Resource.Id.more_info_btn);
        }
    }
}
