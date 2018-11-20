using Edison.Devices.Onboarding.Client.Models;
using Edison.Devices.Onboarding.Common.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Client.Services
{
    public class DeviceProvisioningRestService : RestBaseService
    {
        public DeviceProvisioningRestService(string restServiceUrl, string token)
            : base(restServiceUrl, token)
        {
        }

        public async Task<DeviceCertificateModel> GenerateCertificate(DeviceCertificateRequestModel deviceRequestObj)
        {
            RestRequest request = await PrepareQuery("Certificates", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(deviceRequestObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<DeviceCertificateModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                Debug.WriteLine($"CreationModel: Error while adding a device: {queryResult.StatusCode}");
            return null;
        }

        public async Task<DeviceSecretKeysModel> GetDeviceKeys(Guid deviceId)
        {
            RestRequest request = await PrepareQuery("Security/{deviceId}", Method.GET);
            request.AddUrlSegment("deviceId", deviceId);
            var queryResult = await _client.ExecuteTaskAsync<DeviceSecretKeysModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                Debug.WriteLine($"GetDeviceKeys: Error while retrieving device keys: {queryResult.StatusCode}");
            return null;
        }

        public async Task<DeviceSecretKeysModel> GenerateDeviceKeys(Guid deviceId)
        {
            RestRequest request = await PrepareQuery("Security", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(deviceId), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<DeviceSecretKeysModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                Debug.WriteLine($"GenerateDeviceKeys: Error while generating or retrieving device keys: {queryResult.StatusCode}");
            return null;
        }
    }
}
