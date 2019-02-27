using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Android.Content;
using Android.Views;
using Android.Content.Res;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Edison.Mobile.User.Client.Droid.Holders;
using Edison.Mobile.User.Client.Core.CollectionItemViewModels;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.Android.Common;
using Android.App;
using System.Collections.Generic;
using Edison.Core.Common.Models;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;

namespace Edison.Mobile.User.Client.Droid.Adapters
{
    public class EventDetailsAdapter : RecyclerView.Adapter
    {
        private Context context;
        public List<NotificationModel> Notifications;

        public override int ItemCount => Notifications.Count;

        public EventDetailsAdapter(Context context, List<NotificationModel> notifications )
        {
            this.context = context;
            this.Notifications = notifications;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            EventDetailViewHolder vh = holder as EventDetailViewHolder;

            var notification = Notifications[position];
            var color = position == 0 ? ContextCompat.GetColor(context, Resource.Color.red) : ContextCompat.GetColor(context, Resource.Color.white);

            if (notification != null)
            {
                Drawable background = (Drawable)vh.Dot.Background;
                ((GradientDrawable)background).SetColor(color);

                vh.MessageTime.Text = $"Message - {notification.CreationDate}";
                vh.Message.Text = notification.NotificationText;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            View cell = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.event_detail_cell, parent, false);
            // Create a ViewHolder to find and hold these view references, and register OnClick with the view holder:
            EventDetailViewHolder eventDetailViewHolder = new EventDetailViewHolder(cell);
            return eventDetailViewHolder;
        }
        
    }
}
