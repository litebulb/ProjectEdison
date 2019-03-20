using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Models;
using Edison.Mobile.Admin.Client.Droid.Activities;

namespace Edison.Mobile.Admin.Client.Droid.Adapters
{
    public class WifiSelectionDeviceAdapter : RecyclerView.Adapter
    {
        public List<AvailableNetwork> _devices;
        public Action<string> _action;
        public WifiSelectionDeviceAdapter(List<AvailableNetwork> devices, Action<string> action)
        {
            _devices = devices;
            _action = action;
        }

        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.device_wifi_view, parent, false);
            WifiSelectionDeviceHolder vh = new WifiSelectionDeviceHolder(itemView);
            return vh;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            WifiSelectionDeviceHolder vh = holder as WifiSelectionDeviceHolder;

            var device = _devices[position];

            vh.Caption.Text = device.SSID;
         
            vh.ItemView.Click += (sender, e) =>
            {
                _action(device.SSID);
            };
        }
        

        public override int ItemCount
        {
            get { return _devices.Count; }
        }

    }

    public class WifiSelectionDeviceHolder : RecyclerView.ViewHolder
    {
        public TextView Caption { get; private set; }

        public WifiSelectionDeviceHolder(View itemView) : base(itemView)
        {
            Caption = itemView.FindViewById<TextView>(Resource.Id.deviceWifiSSIDTextView);
        }
        
    }
}
