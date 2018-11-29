using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface IDeviceRestService
    {
        Task<DeviceModel> CreateOrUpdateDevice(DeviceTwinModel device);
        Task<DeviceModel> GetMobileDeviceFromUserId(string userId);
        Task<IEnumerable<Guid>> GetDevicesInRadius(DeviceGeolocationModel deviceGeocodeCenterUpdate);
        Task<bool> IsInBoundaries();
        Task<DeviceHeartbeatUpdatedModel> UpdateHeartbeat(Guid deviceId);
        Task<bool> UpdateGeolocation(DeviceGeolocationUpdateModel updateGeolocationObj);
        Task<bool> DeleteDevice(Guid deviceId);
    }
}
