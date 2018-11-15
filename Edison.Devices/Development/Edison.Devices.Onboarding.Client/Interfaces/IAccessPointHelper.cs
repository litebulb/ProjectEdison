using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Edison.Devices.Onboarding.Client.Models;

namespace Edison.Devices.Onboarding.Client.Interfaces
{
    public interface IAccessPointHelper
    {
        Task<IEnumerable<AccessPoint>> FindAccessPoints();
        Task<WifiConnectionStatus> ConnectToAccessPoint(AccessPoint accessPoint);
        void Disconnect();
    }
}
