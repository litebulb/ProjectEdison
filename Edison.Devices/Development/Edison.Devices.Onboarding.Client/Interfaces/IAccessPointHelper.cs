using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Edison.Devices.Onboarding.Client.Models;

namespace Edison.Devices.Onboarding.Client.Interfaces
{
    public interface IAccessPointHelper
    {
        event Action<string> AccessPointsEnumeratedEvent;
        event Action<string> AccessPointConnectedEvent;

        Task FindAccessPoints(ObservableCollection<AccessPoint> availableAccessPoints);
        Task ConnectToAccessPoint(AccessPoint accessPoint);
        void Disconnect();
    }
}
