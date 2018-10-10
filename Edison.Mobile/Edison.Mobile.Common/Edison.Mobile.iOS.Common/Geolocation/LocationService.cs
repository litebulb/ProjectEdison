using System;
using System.Threading.Tasks;
using CoreLocation;
using Edison.Mobile.Common.Geolocation;
using Edison.Mobile.Common.Logging;
using Foundation;
using UIKit;

namespace Edison.Mobile.iOS.Common.LocationServices
{
    public class LocationService : CLLocationManagerDelegate, ILocationService
    {
        readonly CLLocationManager locationManager = new CLLocationManager();
        readonly ILogger logger;

        public LocationService(ILogger logger)
        {
            this.logger = logger;
            locationManager.Delegate = this;
        }

        public event EventHandler<LocationChangedEventArgs> OnLocationChanged;

        public override void AuthorizationChanged(CLLocationManager manager, CLAuthorizationStatus status)
        {
            logger.Log($"iOS Authorization status changed: {status}");

            if (!(status == CLAuthorizationStatus.Authorized || status == CLAuthorizationStatus.AuthorizedAlways) && status != CLAuthorizationStatus.NotDetermined)
            {
                var alertController = UIAlertController.Create(null, "We couldn't gain access to your location!", UIAlertControllerStyle.Alert);
                alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alertController, true, null);
            }
        }

        public override void UpdatedLocation(CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation) => OnLocationChanged?.Invoke(this, new LocationChangedEventArgs
        {
            CurrentLocation = EdisonLocationFromCLLocation(newLocation),
            LastLocation = oldLocation != null ? EdisonLocationFromCLLocation(oldLocation) : null,
        });

        public async Task<bool> LocationEnabled()
        {
            return await Task.FromResult(CLLocationManager.LocationServicesEnabled);
        }

        public async Task StartLocationUpdates()
        {
            await Task.Run((Action)locationManager.StartUpdatingLocation);
        }

        public async Task StopLocationUpdates()
        {
            await Task.Run((Action)locationManager.StopUpdatingLocation);
        }

        public void RequestLocationPrivileges()
        {
            if (CLLocationManager.Status != CLAuthorizationStatus.AuthorizedAlways)
            {
                locationManager.RequestAlwaysAuthorization();
            }
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
    }
}
