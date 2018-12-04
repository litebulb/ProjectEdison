using System;
using Android.Gms.Location;
using Android.Locations;

namespace Edison.Mobile.Android.Common.Geolocation
{
    public class EdisonLocationCallback : LocationCallback
    {
        public EventHandler<Location> LocationUpdated;

        public override void OnLocationResult(LocationResult result)
        {
            base.OnLocationResult(result);
            LocationUpdated?.Invoke(this, result.LastLocation);
        }
    }
}
