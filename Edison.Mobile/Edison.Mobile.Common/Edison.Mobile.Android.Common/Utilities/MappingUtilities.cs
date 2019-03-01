using System;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Edison.Mobile.Common.Geo;


namespace Edison.Mobile.Android.Common
{
    public static class MappingUtilities
    {

        public const int UserLocationJitterThreshold = 3; //meters
        public const int SingleLocationRefocusMapThreshold = 5000; // meters
        public const float LocationThresholdPercent = 0.1f; // % as fraction

        public static bool RemoveLocationJitter(EdisonLocation oldLocation, EdisonLocation newLocation, int jitterThresholdMeters = UserLocationJitterThreshold)
        {
            if (newLocation == null) return false;
            if (oldLocation == null) return true;
            var latDelta = Math.Abs(newLocation.Latitude - oldLocation.Latitude);
            var longDelta = Math.Abs(newLocation.Longitude - oldLocation.Longitude);
            // Check for user location jitter check the user has moved more than the threshold to update map
            var latDistanceDelta = latDelta * 111111;
            var longDistanceDelta = longDelta * 111111 * Math.Cos(newLocation.Latitude);
            var deltaDist = Math.Sqrt(latDistanceDelta * latDistanceDelta + longDistanceDelta * longDistanceDelta);
            return deltaDist >= jitterThresholdMeters;
        }

        public static bool ShouldMoveMap1(LatLng newUserLocation, LatLng oldUserLocation, LatLng eventLocation,
                           float locationThresholdPercent = LocationThresholdPercent,
                           int singleLocationRefocusMapThreshold = SingleLocationRefocusMapThreshold)
        {
            var userLatDelta = Math.Abs(newUserLocation.Latitude - oldUserLocation.Latitude);
            var userLongDelta = Math.Abs(newUserLocation.Longitude - oldUserLocation.Longitude);
            var userLatDistanceDelta = userLatDelta * 111111;
            var userLongDistanceDelta = userLongDelta * 111111 * Math.Cos(newUserLocation.Latitude);
            var userDeltaDist = Math.Sqrt(userLatDistanceDelta * userLatDistanceDelta + userLongDistanceDelta * userLongDistanceDelta);
            if (eventLocation == null)
                // if eventLocation is null, then Only User Location is available, so use single location threshold
                // Check to see if location has moved enough to move the map
                return userDeltaDist >= singleLocationRefocusMapThreshold;
            else
            {
                // both event and user location are available
                // use a user movement threshold that is % of distance between the user and event
                var latDelta = Math.Abs(eventLocation.Latitude - newUserLocation.Latitude);
                var longDelta = Math.Abs(eventLocation.Longitude - newUserLocation.Longitude);
                var latDistanceDelta = latDelta * 111111;
                var longDistanceDelta = longDelta * 111111 * Math.Cos(newUserLocation.Latitude);
                var deltaDist = Math.Sqrt(latDistanceDelta * latDistanceDelta + longDistanceDelta * longDistanceDelta);
                // Check to see if user location has moved enough to move the map
                return userDeltaDist >= deltaDist * locationThresholdPercent;
            }
        }
    }

    public class MapLoadedCallback : Java.Lang.Object, GoogleMap.IOnMapLoadedCallback
    {
        public static EventHandler<int> MapLoaded;

        private int _mapViewResId;

        public MapLoadedCallback(int mapViewResId)
        {
            _mapViewResId = mapViewResId;
        }

        void GoogleMap.IOnMapLoadedCallback.OnMapLoaded()
        {
            MapLoaded?.Invoke(null, _mapViewResId);
        }
    }
}