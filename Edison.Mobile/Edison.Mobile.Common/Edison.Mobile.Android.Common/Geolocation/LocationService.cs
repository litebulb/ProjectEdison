using System;
using System.Threading.Tasks;
using Android.App;
using Android.Locations;
using Android.Gms.Common;
using Android.Gms.Location;
using Edison.Mobile.Common.Geolocation;
using Android.OS;
using Android;
using Android.Content.PM;
using Android.Support.V4.Content;
using Android.Support.V4.App;

namespace Edison.Mobile.Android.Common.Geolocation
{
    public class LocationService : Activity, ILocationService
    {
        readonly FusedLocationProviderClient locationProvider;
        readonly EdisonLocationCallback locationCallback;

        Location lastLocation;

        public LocationService()
        {
            locationProvider = LocationServices.GetFusedLocationProviderClient(Application.Context);
            locationCallback = new EdisonLocationCallback();
            locationCallback.LocationUpdated += OnLocationUpdated;
        }

        public event EventHandler<Mobile.Common.Geolocation.LocationChangedEventArgs> OnLocationChanged;

        public async Task<bool> LocationEnabled()
        {
            var availability = await locationProvider.GetLocationAvailabilityAsync();
            return availability.IsLocationAvailable;
        }

        public void RequestLocationPrivileges()
        {
            var permission = Manifest.Permission.AccessFineLocation;
            if (ContextCompat.CheckSelfPermission(locationProvider.ApplicationContext, permission) == Permission.Granted)
            {
                return;
            }

            ActivityCompat.RequestPermissions(this, new string[] { permission }, 0);
        }

        public async Task StartLocationUpdates()
        {
            try
            {
                var locationRequest = new LocationRequest()
                    .SetInterval(5000)
                    .SetFastestInterval(5000)
                    .SetPriority(LocationRequest.PriorityHighAccuracy);

                await locationProvider.RequestLocationUpdatesAsync(locationRequest, locationCallback);
            }
            catch (Exception e)
            {
                // TODO: log, tell user
            }
        }

        public async Task StopLocationUpdates()
        {
            await locationProvider.RemoveLocationUpdatesAsync(locationCallback);
        }

        void OnLocationUpdated(object sender, Location location)
        {
            OnLocationChanged?.Invoke(this, new Mobile.Common.Geolocation.LocationChangedEventArgs
            {
                CurrentLocation = location != null ? EdisonLocationFromLocation(location) : null,
                LastLocation = lastLocation != null ? EdisonLocationFromLocation(lastLocation) : null,
            });

            lastLocation = location;
        }

        bool IsGooglePlayServicesInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);

                // TODO: log, tell user
            }

            return false;
        }

        EdisonLocation EdisonLocationFromLocation(Location location) => new EdisonLocation
        {
            Altitude = location.Altitude,
            Bearing = location.Bearing,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Speed = location.Speed,
            Time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(location.Time),
        };
    }
}
