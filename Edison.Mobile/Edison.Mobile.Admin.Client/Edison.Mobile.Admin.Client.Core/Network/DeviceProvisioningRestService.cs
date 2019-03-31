using System;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.Models;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Network;
using RestSharp;

namespace Edison.Mobile.Admin.Client.Core.Network
{
    public class DeviceProvisioningRestService : BaseRestService, IDeviceProvisioningRestService
    {
        public DeviceProvisioningRestService(AuthService authService, ILogger logger, string baseUrl)
            : base(authService, logger, baseUrl)
        {
        }

        public async Task<DeviceCertificateModel> GenerateDeviceCertificate(DeviceCertificateRequestModel deviceCertificateRequestModel)
        {
            var request = PrepareRequest("/Certificates", Method.POST, deviceCertificateRequestModel);
            var result = await client.ExecutePostTaskAsync<DeviceCertificateModel>(request);
            if (result.IsSuccessful)
            {
                return result.Data;
            }

            logger.Log($"Error generating device certificate. Status code: {result.StatusCode}, Error Message: {result.ErrorMessage}");

            return null;
        }

        public async Task<DeviceSecretKeysModel> GenerateDeviceKeys(Guid deviceId, string ssidName)
        {
            var requestObject = new
            {
                DeviceId = deviceId,
                SSIDName = ssidName
            };

            var request = PrepareRequest("Security", Method.POST, requestObject);
            var queryResult = await client.ExecutePostTaskAsync<DeviceSecretKeysModel>(request);
            if (queryResult.IsSuccessful)
            {
                return queryResult.Data;
            }

            logger.Log($"GenerateDeviceKeys: Error while generating or retrieving device keys: {queryResult.StatusCode}");

            return null;
        }

        public async Task<DeviceSecretKeysModel> GetDeviceKeys(Guid deviceId)
        {
            RestRequest request = PrepareRequest("Security/{deviceId}", Method.GET);
            request.AddUrlSegment("deviceId", deviceId);
            var queryResult = await client.ExecuteGetTaskAsync<DeviceSecretKeysModel>(request);
            if (queryResult.IsSuccessful)
            {
                return queryResult.Data;
            }

            logger.Log($"GetDeviceKeys: Error while retrieving device keys: {queryResult.StatusCode}");

            return null;
        }
    }
}
