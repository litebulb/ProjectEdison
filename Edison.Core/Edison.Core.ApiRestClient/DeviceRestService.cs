using Edison.Core.Common.Models;
using Edison.Core.Config;
using Edison.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Edison.Core
{
    public class DeviceRestService : RestServiceBase, IDeviceRestService
    {
        public DeviceRestService(IOptions<RestServiceOptions> config, ILogger<EventClusterRestService> logger)
            : base(config.Value.AzureAd, config.Value.RestServiceUrl, logger)
        {
        }

        public DeviceRestService(string authority, string tenantId, string clientId, string clientSecret, string restServiceUrl, ILogger<RestServiceBase> logger)
            : base(authority, tenantId, clientId, clientSecret, restServiceUrl, logger)
        {
        }

        public async Task<IEnumerable<Guid>> GetDevicesInRadius(DeviceGeolocationModel deviceGeocodeCenterUpdate)
        {
            RestRequest request = await PrepareQuery("Devices/Radius", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(deviceGeocodeCenterUpdate), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<IEnumerable<Guid>>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetDevicesInRadius: Error while adding an event: {queryResult.StatusCode}");
            return null;
        }

        public async Task<DeviceModel> CreateOrUpdateDevice(DeviceTwinModel deviceObj)
        {
            RestRequest request = await PrepareQuery("Devices", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(deviceObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<DeviceModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"CreationModel: Error while adding a device: {queryResult.StatusCode}");
            return null;
        }

        public async Task<bool> UpdateHeartbeat(Guid deviceId)
        {
            RestRequest request = await PrepareQuery("Devices/Heartbeat", Method.PUT);
            request.AddParameter("application/json", JsonConvert.SerializeObject(deviceId), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync(request);
            if (!queryResult.IsSuccessful)
            {
                _logger.LogError($"UpdateHeartbeat: Error while updating heartbeat: {queryResult.StatusCode}");
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteDevice(Guid deviceId)
        {
            RestRequest request = await PrepareQuery("Devices", Method.DELETE);
            request.AddParameter("deviceId", deviceId.ToString());

            var queryResult = await _client.ExecuteTaskAsync(request);
            if (!queryResult.IsSuccessful)
            {
                _logger.LogError($"DeleteDevice: Error while sending a message: {queryResult.StatusCode}");
                return false;
            }
            return true;
        }
    }
}
