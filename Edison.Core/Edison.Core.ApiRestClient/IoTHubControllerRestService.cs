using Edison.Core.Common.Models;
using Edison.Core.Config;
using Edison.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;

namespace Edison.Core
{
    public class IoTHubControllerRestService : RestServiceBase, IIoTHubControllerRestService
    {
        public IoTHubControllerRestService(IOptions<RestServiceOptions> config, ILogger<EventClusterRestService> logger)
            : base(config.Value.AzureAd, config.Value.RestServiceUrl, logger)
        {
        }

        public IoTHubControllerRestService(string authority, string tenantId, string clientId, string clientSecret, string restServiceUrl, ILogger<RestServiceBase> logger)
            : base(authority, tenantId, clientId, clientSecret, restServiceUrl, logger)
        {
        }

        public async Task<bool> UpdateDevicesDesired(DevicesUpdateDesiredModel devices)
        {
            RestRequest request = await PrepareQuery("IoTHub/Desired", Method.PUT);
            request.AddParameter("application/json", JsonConvert.SerializeObject(devices), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync(request);
            if (!queryResult.IsSuccessful)
            {
                _logger.LogError($"UpdateDevicesDesired: Error while updating heartbeat: {queryResult.StatusCode}");
                return false;
            }
            return true;
        }
    }
}
