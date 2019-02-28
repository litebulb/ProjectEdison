using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Edison.Core.Common.Models;

namespace Edison.Mobile.Admin.Client.Droid.Adapters
{
    public class WifiSelectionDeviceAdapter : RecyclerView.Adapter
    {
        public List<Common.WiFi.WifiNetwork> _devices;
        public WifiSelectionDeviceAdapter(List<Common.WiFi.WifiNetwork> devices)
        {
            _devices = devices;
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
