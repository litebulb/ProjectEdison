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
        public Action<Guid> _action;

        public NearbyDeviceAdapter(List<DeviceModel> devices, Action<Guid> action)
        {
            _devices = devices;
            _action = action;
        }

        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.nearby_device_item_view, parent, false);
            NearbyDeviceHolder vh = new NearbyDeviceHolder(itemView, _action);
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
            vh.DeviceId = device.DeviceId;
        }

        public override int ItemCount
        {
            get { return _devices.Count; }
        }
    }

    public class NearbyDeviceHolder : RecyclerView.ViewHolder, View.IOnClickListener
    {
        
        public Action<Guid> _action;

        public ImageView DeviceTypeImage { get; private set; }
        public LinearLayout StateLayout { get; private set; }
        public TextView Caption { get; private set; }
        public Guid DeviceId { get; set; }

        public NearbyDeviceHolder(View itemView, Action<Guid> action) : base(itemView)
        {
            itemView.SetOnClickListener(this);
            // Locate and cache view references:
            DeviceTypeImage = itemView.FindViewById<ImageView>(Resource.Id.typeImageView);
            StateLayout = itemView.FindViewById<LinearLayout>(Resource.Id.stateLayout);
            Caption = itemView.FindViewById<TextView>(Resource.Id.textView);
            _action = action;
        }

        public void OnClick(View v)
        {
            _action(DeviceId);
        }
    }
}
