using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Support.V4.Content.Res;
using Android.Support.V4.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Common.Geo;

namespace Edison.Mobile.User.Client.Droid.Holders
{
    public class EventDetailViewHolder : RecyclerView.ViewHolder
    {
        public View Dot { get; private set; }
        public TextView MessageTime { get; private set; }
        public TextView Message { get; private set; }

        public EventDetailViewHolder(View item) : base(item) 
        {
            Dot = item.FindViewById<View>(Resource.Id.left_dot);
            MessageTime = item.FindViewById<TextView>(Resource.Id.event_cell_time);
            Message = item.FindViewById<TextView>(Resource.Id.event_cell_message);
        }
    }
}
