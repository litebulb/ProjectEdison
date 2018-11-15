using Edison.Devices.Onboarding.Client.Models;
using Edison.Devices.Onboarding.Common.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Client.Services
{
    public class DeviceProvisioningRestService : RestServiceBase
    {
        public DeviceProvisioningRestService(string restServiceUrl, string token)
            : base(restServiceUrl, token)
        {
        }

        public async Task<DeviceCertificateModel> GenerateCertificate(DeviceCertificateRequestModel deviceRequestObj)
        {
            RestRequest request = PrepareQuery("Certificates", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(deviceRequestObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<DeviceCertificateModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                Debug.WriteLine($"CreationModel: Error while adding a device: {queryResult.StatusCode}");
            return null;
        }
    }
}
