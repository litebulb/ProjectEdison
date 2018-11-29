using Edison.Core.Common.Models;
using Edison.Core.Config;
using Edison.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edison.Core
{
    public class EventClusterRestService : RestServiceBase, IEventClusterRestService
    {
        public EventClusterRestService(IOptions<RestServiceOptions> config, ILogger<EventClusterRestService> logger)
            : base(config.Value.AzureAd, config.Value.RestServiceUrl, logger)
        {
        }

        public EventClusterRestService(string authority, string tenantId, string clientId, string clientSecret, string restServiceUrl, ILogger<RestServiceBase> logger)
            : base(authority, tenantId, clientId, clientSecret, restServiceUrl, logger)
        {
        }

        public async Task<EventClusterModel> GetEventCluster(Guid eventClusterId)
        {
            RestRequest request = await PrepareQuery("EventClusters/{eventClusterId}", Method.GET);
            request.AddUrlSegment("eventClusterId", eventClusterId);
            var queryResult = await _client.ExecuteTaskAsync<EventClusterModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetEventCluster: {queryResult.StatusCode}");
            return null;
        }

        public async Task<IEnumerable<EventClusterModel>> GetEventClusters()
        {
            RestRequest request = await PrepareQuery("EventClusters", Method.GET);
            var queryResult = await _client.ExecuteTaskAsync<IEnumerable<EventClusterModel>>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetEventClusters: {queryResult.StatusCode}");
            return null;
        }

        public async Task<IEnumerable<Guid>> GetClustersInRadius(EventClusterGeolocationModel eventClusterGeocodeCenterUpdate)
        {
            RestRequest request = await PrepareQuery("EventClusters/Radius", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(eventClusterGeocodeCenterUpdate), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<IEnumerable<Guid>>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetEventClusters: Error while adding an event: {queryResult.StatusCode}");
            return null;
        }

        public async Task<EventClusterModel> CreateOrUpdateEventCluster(EventClusterCreationModel eventObj)
        {
            RestRequest request = await PrepareQuery("EventClusters", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(eventObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<EventClusterModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"CreateOrUpdateEventClusterFromIoTDevice: Error while adding an event: {queryResult.StatusCode}");
            return null;
        }

        public async Task<EventClusterModel> CloseEventCluster(EventClusterCloseModel eventObj)
        {
            RestRequest request = await PrepareQuery("EventClusters/Close", Method.PUT);
            request.AddParameter("application/json", JsonConvert.SerializeObject(eventObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<EventClusterModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"CloseEventCluster: Error while closing an event cluster: {queryResult.StatusCode}");
            return null;
        }
    }
}
