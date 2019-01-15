using System;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.Models;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Network;
using RestSharp;

namespace Edison.Mobile.Admin.Client.Core.Network
{
    public class DeviceProvisioningRestService : BaseRestService
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
    }
}
