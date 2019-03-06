using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Core.Common.Models;

namespace Edison.Mobile.Admin.Client.Core.Ioc
{
    public interface IDeviceRestService
    {
        Task<DeviceModel> GetDevice(Guid deviceId);
        Task<IEnumerable<DeviceModel>> GetDevices(Geolocation geolocation = null);
        Task<bool> UpdateDevice(DevicesUpdateTagsModel updateTagsModel);
    }
}