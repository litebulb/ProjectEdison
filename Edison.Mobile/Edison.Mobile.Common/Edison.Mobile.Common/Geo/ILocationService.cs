using System;
using System.Threading.Tasks;

namespace Edison.Mobile.Common.Geo
{
    public class LocationChangedEventArgs : EventArgs 
    {
        public EdisonLocation CurrentLocation;
        public EdisonLocation LastLocation;
    }

    public interface ILocationService
    {
        Task<bool> LocationEnabled();
        Task<bool> HasLocationPrivileges();
        Task StartLocationUpdates();
        Task StopLocationUpdates();
        Task<bool> RequestLocationPrivileges();
        event EventHandler<LocationChangedEventArgs> OnLocationChanged;
        EdisonLocation LastKnownLocation { get; }
    }
}
