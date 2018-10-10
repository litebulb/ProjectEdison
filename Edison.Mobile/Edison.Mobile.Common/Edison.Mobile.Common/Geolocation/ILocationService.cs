using System;
using System.Threading.Tasks;

namespace Edison.Mobile.Common.Geolocation
{
    public class LocationChangedEventArgs : EventArgs 
    {
        public EdisonLocation CurrentLocation;
        public EdisonLocation LastLocation;
    }

    public interface ILocationService
    {
        Task<bool> LocationEnabled();
        Task StartLocationUpdates();
        Task StopLocationUpdates();
        void RequestLocationPrivileges();
        event EventHandler<LocationChangedEventArgs> OnLocationChanged;
    }
}
