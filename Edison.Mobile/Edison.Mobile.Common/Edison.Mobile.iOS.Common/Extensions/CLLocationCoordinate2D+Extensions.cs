using System;
using CoreLocation;

namespace Edison.Mobile.iOS.Common.Extensions
{
    public static class CLLocationCoordinate2DExtensions
    {
        public static CLLocationCoordinate2D GetMidpointCoordinate(this CLLocationCoordinate2D coordinate, CLLocationCoordinate2D secondLocation)
        {
            var lat1 = coordinate.Latitude * Math.PI / 180;
            var lon1 = coordinate.Longitude * Math.PI / 180;

            var lat2 = secondLocation.Latitude * Math.PI / 180;
            var lon2 = secondLocation.Longitude * Math.PI / 180;

            var lonDelta = lon2 - lon1;

            var x = Math.Cos(lat2) * Math.Cos(lonDelta);
            var y = Math.Cos(lat2) * Math.Sin(lonDelta);

            var lat3 = Math.Atan2(Math.Sin(lat1) + Math.Sin(lat2), Math.Sqrt((Math.Cos(lat1) + x) * (Math.Cos(lat1) + x) + y * y));
            var lon3 = lon1 + Math.Atan2(y, Math.Cos(lat1) + x);

            var midpointCoordinate = new CLLocationCoordinate2D(lat3 * 180 / Math.PI, lon3 * 180 / Math.PI);
            return midpointCoordinate;
        }
    }
}
