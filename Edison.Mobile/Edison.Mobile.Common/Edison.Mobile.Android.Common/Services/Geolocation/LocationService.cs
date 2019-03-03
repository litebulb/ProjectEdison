using System;
using System.Threading.Tasks;
using Android.App;
using Android.Locations;
using Android.Gms.Common;
using Android.Gms.Location;
using Edison.Mobile.Common.Geo;
using Android.OS;
using Android;
using Android.Content.PM;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using System.Threading;

namespace Edison.Mobile.Android.Common.Geolocation
{
    [System.Runtime.InteropServices.Guid("68F3FCE1-D861-4290-A8D7-1AF30D3C22E3")]
    public class LocationService : ILocationService
    {

        public event EventHandler<Mobile.Common.Geo.LocationChangedEventArgs> OnLocationChanged;

        private const int RequestLocationPermissionId = 1;

        private static FusedLocationProviderClient _locationProvider = null;
        private static EdisonLocationCallback _locationCallback = null;

        private static bool _locationUpdatesStarted = false;


        private Location lastLocation;

        public EdisonLocation LastKnownLocation => EdisonLocationFromLocation(lastLocation);

        public LocationService()
        {
            //            this.mainActivity = mainActivity;
            if (_locationProvider == null)
                _locationProvider = LocationServices.GetFusedLocationProviderClient(Application.Context);
            if (_locationCallback == null)
                _locationCallback = new EdisonLocationCallback();

            

            _locationCallback.LocationUpdated += OnLocationUpdated;
        }



        public async Task<bool> LocationEnabled()
        {
            var availability = await _locationProvider.GetLocationAvailabilityAsync();
            return availability.IsLocationAvailable;
        }

        public async Task<bool> RequestLocationPrivileges()
        {
            var permission = Manifest.Permission.AccessFineLocation;
            if (ContextCompat.CheckSelfPermission(_locationProvider.ApplicationContext, permission) == Permission.Granted)
            {
                return true;
            }

            //         ActivityCompat.RequestPermissions(this.mainActivity, new string[] { permission }, RequestLocationPermissionId);
            ActivityCompat.RequestPermissions(BaseApplication.CurrentActivity, new string[] { permission }, RequestLocationPermissionId);
            return await Task.FromResult(ContextCompat.CheckSelfPermission(_locationProvider.ApplicationContext, permission) == Permission.Granted);
        }


        public async Task StartLocationUpdates()
        {
            if (!_locationUpdatesStarted)
            { 
                try
                {
                    var locationRequest = new LocationRequest()
                        .SetInterval(5000)
                        .SetFastestInterval(5000)
                        .SetPriority(LocationRequest.PriorityHighAccuracy);

                    await _locationProvider.RequestLocationUpdatesAsync(locationRequest, _locationCallback);
                    _locationUpdatesStarted = true;
                }
                catch (Exception e)
                {
                    _locationUpdatesStarted = false;
                    // TODO: log, tell user
                }
            }
        }

        public async Task StopLocationUpdates()
        {
            await _locationProvider.RemoveLocationUpdatesAsync(_locationCallback);
            _locationUpdatesStarted = false;
        }

        private void OnLocationUpdated(object sender, Location location)
        {
            OnLocationChanged?.Invoke(this, new Mobile.Common.Geo.LocationChangedEventArgs
            {
                CurrentLocation = location != null ? EdisonLocationFromLocation(location) : null,
                LastLocation = lastLocation != null ? EdisonLocationFromLocation(lastLocation) : null,
            });

            lastLocation = location;
        }

        bool IsGooglePlayServicesInstalled()
        {
            //           var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(BaseApplication.CurrentActivity);
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

        EdisonLocation EdisonLocationFromLocation(Location location)
        {
            return location == null ? null : new EdisonLocation
                                                                {
                                                                    Altitude = location.Altitude,
                                                                    Bearing = location.Bearing,
                                                                    Latitude = location.Latitude,
                                                                    Longitude = location.Longitude,
                                                                    Speed = location.Speed,
                                                                    Time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(location.Time),
                                                                };
        }
            

        public async Task<EdisonLocation> GetLastKnownLocationAsync()
        {
            var location = await _locationProvider.GetLastLocationAsync();
            return EdisonLocationFromLocation(location);
        }



        public Task<bool> HasLocationPrivileges()
        {
            return Task.FromResult(ContextCompat.CheckSelfPermission(_locationProvider.ApplicationContext, Manifest.Permission.AccessFineLocation) == Permission.Granted);
        }
    }
}
