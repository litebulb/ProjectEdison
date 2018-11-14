using Edison.Core.Common.Models;
using Edison.Core.Config;
using Edison.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Threading.Tasks;

namespace Edison.Core
{
    public class SignalRRestService : RestServiceBase, ISignalRRestService
    {
        public SignalRRestService(IOptions<RestServiceOptions> config, ILogger<SignalRRestService> logger)
            : base(config.Value.AzureAd, config.Value.RestServiceUrl, logger)
        {
        }

        public SignalRRestService(string authority, string tenantId, string clientId, string clientSecret, string restServiceUrl, ILogger<RestServiceBase> logger)
            : base(authority, tenantId, clientId, clientSecret, restServiceUrl, logger)
        {
        }

        public async Task<bool> UpdateEventClusterUI(EventClusterUIModel eventClusterUIUpdate)
        {
            RestRequest request = await PrepareQuery("SignalR/EventCluster", Method.PUT);
            request.AddParameter("application/json", JsonConvert.SerializeObject(eventClusterUIUpdate), ParameterType.RequestBody);

            var queryResult = await _client.ExecuteTaskAsync(request);
            if (!queryResult.IsSuccessful)
            {
                _logger.LogError($"UpdateEventClusterUI: Error while sending a message: {queryResult.StatusCode}");
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateDeviceUI(DeviceUIModel deviceUIUpdate)
        {
            RestRequest request = await PrepareQuery("SignalR/Device", Method.PUT);
            request.AddParameter("application/json", JsonConvert.SerializeObject(deviceUIUpdate), ParameterType.RequestBody);

            var queryResult = await _client.ExecuteTaskAsync(request);
            if (!queryResult.IsSuccessful)
            {
                _logger.LogError($"UpdateDeviceUI: Error while sending a message: {queryResult.StatusCode}");
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateResponseUI(ResponseUIModel responseUIUpdate)
        {
            RestRequest request = await PrepareQuery("SignalR/Response", Method.PUT);
            request.AddParameter("application/json", JsonConvert.SerializeObject(responseUIUpdate), ParameterType.RequestBody);

            var queryResult = await _client.ExecuteTaskAsync(request);
            if (!queryResult.IsSuccessful)
            {
                _logger.LogError($"UpdateResponseUI: Error while sending a message: {queryResult.StatusCode}");
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateActionCloseUI(ActionCloseUIModel actionCloseUIUpdate)
        {
            RestRequest request = await PrepareQuery("SignalR/Response/ActionClose", Method.PUT);
            request.AddParameter("application/json", JsonConvert.SerializeObject(actionCloseUIUpdate), ParameterType.RequestBody);

            var queryResult = await _client.ExecuteTaskAsync(request);
            if (!queryResult.IsSuccessful)
            {
                _logger.LogError($"UpdateActionCloseUI: Error while sending a message: {queryResult.StatusCode}");
                return false;
            }
            return true;
        }
    }
}
