using System;
using System.Threading.Tasks;
using CoreLocation;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.Common.Logging;
using Foundation;
using UIKit;

namespace Edison.Mobile.iOS.Common.LocationServices
{
    public class LocationService : CLLocationManagerDelegate, ILocationService
    {
        readonly CLLocationManager locationManager = new CLLocationManager();
        readonly ILogger logger;

        TaskCompletionSource<bool> requestPrivilegesTaskCompletionSource;

        public EdisonLocation LastKnownLocation { get; private set; }

        public LocationService(ILogger logger)
        {
            this.logger = logger;
            locationManager.Delegate = this;
        }

        public event EventHandler<LocationChangedEventArgs> OnLocationChanged;

        public override void AuthorizationChanged(CLLocationManager manager, CLAuthorizationStatus status)
        {
            logger.Log($"iOS Authorization status changed: {status}");

            if (!(status == CLAuthorizationStatus.AuthorizedAlways || status == CLAuthorizationStatus.AuthorizedWhenInUse || status == CLAuthorizationStatus.Authorized) && status != CLAuthorizationStatus.NotDetermined)
            {
                var alertController = UIAlertController.Create(null, "We couldn't gain access to your location!", UIAlertControllerStyle.Alert);
                alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alertController, true, null);
                requestPrivilegesTaskCompletionSource?.TrySetResult(true);
            }

            if (status == CLAuthorizationStatus.Authorized || status == CLAuthorizationStatus.AuthorizedAlways || status == CLAuthorizationStatus.AuthorizedWhenInUse)
            {
                requestPrivilegesTaskCompletionSource?.TrySetResult(true);
            }
        }

        public override void UpdatedLocation(CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
        {
            var currentLocation = EdisonLocationFromCLLocation(newLocation);
            OnLocationChanged?.Invoke(this, new LocationChangedEventArgs
            {
                CurrentLocation = currentLocation,
                LastLocation = oldLocation != null ? EdisonLocationFromCLLocation(oldLocation) : null,
            });

            LastKnownLocation = currentLocation;
        }

        public async Task<bool> LocationEnabled()
        {
            return await Task.FromResult(CLLocationManager.LocationServicesEnabled);
        }

        public async Task StartLocationUpdates()
        {
            var hasLocationPrivileges = await HasLocationPrivileges();
            if (CLLocationManager.LocationServicesEnabled && hasLocationPrivileges)
            {
                await Task.Run((Action)locationManager.StartUpdatingLocation);
            }
        }

        public async Task StopLocationUpdates()
        {
            await Task.Run((Action)locationManager.StopUpdatingLocation);
        }

        public Task<bool> RequestLocationPrivileges()
        {
            requestPrivilegesTaskCompletionSource = new TaskCompletionSource<bool>();
            if (CLLocationManager.Status != CLAuthorizationStatus.AuthorizedAlways || CLLocationManager.Status != CLAuthorizationStatus.AuthorizedWhenInUse)
            {
                locationManager.RequestAlwaysAuthorization();
            }
            else
            {
                requestPrivilegesTaskCompletionSource.TrySetResult(true);
            }

            return requestPrivilegesTaskCompletionSource.Task;
        }

        EdisonLocation EdisonLocationFromCLLocation(CLLocation location) => new EdisonLocation
        {
            Altitude = location.Altitude,
            Latitude = location.Coordinate.Latitude,
            Longitude = location.Coordinate.Longitude,
            Speed = location.Speed,
            Time = DateTimeFromNSDate(location.Timestamp),
            Bearing = location.Course,
        };

        DateTime DateTimeFromNSDate(NSDate date)
        {
            DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(2001, 1, 1, 0, 0, 0));
            return reference.AddSeconds(date.SecondsSinceReferenceDate);
        }

        public Task<bool> HasLocationPrivileges()
        {
            return Task.FromResult(CLLocationManager.Status != CLAuthorizationStatus.NotDetermined && CLLocationManager.Status != CLAuthorizationStatus.Denied);
        }

        public async Task<EdisonLocation> GetLastKnownLocationAsync()
        {
           return await Task.Run(() =>
           {
               return LastKnownLocation;
           });
        }
    }
}
