using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Edison.Core.Common.Models;

namespace Edison.Mobile.Admin.Client.Droid.Adapters
{
    public class NearbyDeviceAdapter : RecyclerView.Adapter
    {
        public List<DeviceModel> _devices;
        public NearbyDeviceAdapter(List<DeviceModel> devices)
        {
            _devices = devices;
        }

        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.nearby_device_item_view, parent, false);
            NearbyDeviceHolder vh = new NearbyDeviceHolder(itemView);
            return vh;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            NearbyDeviceHolder vh = holder as NearbyDeviceHolder;

            var device = _devices[position];

            int icon = Resource.Drawable.disabled_circle;
            if(device.Online)
            {
                icon = Resource.Drawable.enabled_circle;
            }
                       
            vh.StateLayout.SetBackgroundResource(icon);
            
            switch (device.DeviceType)
            {
                case "SmartBulb":
                case "ButtonSensor":
                    vh.DeviceTypeImage.SetImageResource(Resource.Drawable.power);
                    break;
                case "SoundSensor":
                    vh.DeviceTypeImage.SetImageResource(Resource.Drawable.lines);
                    break;
            }

            vh.Caption.Text = device.Name;
        }

        public override int ItemCount
        {
            get { return _devices.Count; }
        }
    }

    public class NearbyDeviceHolder : RecyclerView.ViewHolder
    {
        public ImageView DeviceTypeImage { get; private set; }
        public LinearLayout StateLayout { get; private set; }
        public TextView Caption { get; private set; }

        public NearbyDeviceHolder(View itemView) : base(itemView)
        {
            // Locate and cache view references:
            DeviceTypeImage = itemView.FindViewById<ImageView>(Resource.Id.typeImageView);
            StateLayout = itemView.FindViewById<LinearLayout>(Resource.Id.stateLayout);
            Caption = itemView.FindViewById<TextView>(Resource.Id.textView);
        }
    }
}
