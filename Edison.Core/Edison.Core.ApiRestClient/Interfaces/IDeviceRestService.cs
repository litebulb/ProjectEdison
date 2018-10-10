using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface IDeviceRestService
    {
        Task<DeviceModel> CreateOrUpdateDevice(DeviceTwinModel device);
        Task<IEnumerable<Guid>> GetDevicesInRadius(DeviceGeolocationModel deviceGeocodeCenterUpdate);
        Task<bool> UpdateHeartbeat(Guid deviceId);
        Task<bool> DeleteDevice(Guid deviceId);
    }
}
