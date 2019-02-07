using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Models;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Network;
using RestSharp;

namespace Edison.Mobile.Admin.Client.Core.Network
{
    public class DeviceRestService : BaseRestService
    {
        public DeviceRestService(AuthService authService, ILogger logger, string baseUrl)
            : base(authService, logger, baseUrl)
        {
        }

        public async Task<IEnumerable<DeviceModel>> GetDevices()
        {
            var request = PrepareRequest("/Devices", Method.GET);
            var result = await client.ExecuteTaskAsync<IEnumerable<DeviceModel>>(request);
            if (result.IsSuccessful)
            {
                return result.Data;
            }

            logger.Log($"Error getting devices. Status code: {result.StatusCode}, Error Message: {result.ErrorMessage}");

            return null;
        }

        public async Task<DeviceModel> GetDevice(Guid deviceId)
        {
            var request = PrepareRequest("/Devices/{deviceId}", Method.GET);
            request.AddUrlSegment("deviceId", deviceId.ToString());
            var result = await client.ExecuteTaskAsync<DeviceModel>(request);
            if (result.IsSuccessful)
            {
                return result.Data;
            }

            logger.Log($"Error getting devices. Status code: {result.StatusCode}, Error Message: {result.ErrorMessage}");

            return null;
        }

        public async Task<object> UpdateDevice(DeviceModel deviceModel)
        {
            throw new NotImplementedException();
        }
    }
}
