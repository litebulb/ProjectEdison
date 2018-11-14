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
    public class ResponseRestService : RestServiceBase, IResponseRestService
    {
        public ResponseRestService(IOptions<RestServiceOptions> config, ILogger<ResponseRestService> logger)
            : base(config.Value.AzureAd, config.Value.RestServiceUrl, logger)
        {
        }

        public ResponseRestService(string authority, string tenantId, string clientId, string clientSecret, string restServiceUrl, ILogger<RestServiceBase> logger)
            : base(authority, tenantId, clientId, clientSecret, restServiceUrl, logger)
        {
        }

        public async Task<ResponseModel> GetResponseDetail(Guid responseId)
        {
            RestRequest request = await PrepareQuery("Responses/{responseId}", Method.GET);
            request.AddUrlSegment("responseId", responseId);
            var queryResult = await _client.ExecuteTaskAsync<ResponseModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetResponseDetail: {queryResult.StatusCode}");
            return null;
        }
        public async Task<IEnumerable<ResponseLightModel>> GetResponses()
        {
            RestRequest request = await PrepareQuery("Responses", Method.GET);
            var queryResult = await _client.ExecuteTaskAsync<IEnumerable<ResponseLightModel>>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetResponses: {queryResult.StatusCode}");
            return null;
        }
        public async Task<IEnumerable<ResponseModel>> GetResponsesFromPointRadius(ResponseGeolocationModel responseGeolocationObj)
        {
            RestRequest request = await PrepareQuery("Responses/Radius", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(responseGeolocationObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<IEnumerable<ResponseModel>>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetResponsesFromPointRadius: Error: {queryResult.StatusCode}");
            return null;
        }
        public async Task<ResponseModel> CreateResponse(ResponseCreationModel responseObj)
        {
            RestRequest request = await PrepareQuery("Responses", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(responseObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<ResponseModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"CreateResponse: Error while adding a response: {queryResult.StatusCode}");
            return null;
        }
        public async Task<bool> DeleteResponse(Guid responseId)
        {
            RestRequest request = await PrepareQuery("Responses/{responseId}", Method.DELETE);
            request.AddParameter("responseId", responseId.ToString());
            var queryResult = await _client.ExecuteTaskAsync(request);
            if (!queryResult.IsSuccessful)
            {
                _logger.LogError($"DeleteResponse: {queryResult.StatusCode}");
                return false;
            }
            return true;
        }

        public async Task<ResponseModel> CloseResponse(ResponseCloseModel responseObj)
        {
            RestRequest request = await PrepareQuery("Responses/Close", Method.PUT);
            request.AddParameter("application/json", JsonConvert.SerializeObject(responseObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<ResponseModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"CloseResponse: Error while closing a response: {queryResult.StatusCode}");
            return null;
        }
        public async Task<ResponseModel> AddEventClusterIdsToResponse(ResponseEventClustersUpdateModel responseObj)
        {
            RestRequest request = await PrepareQuery("Responses/AddEventClusters", Method.PUT);
            request.AddParameter("application/json", JsonConvert.SerializeObject(responseObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<ResponseModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"AddEventClusterIdsToResponse: Error while updating a response: {queryResult.StatusCode}");
            return null;
        }

        public async Task<ResponseModel> ChangeResponseAction(ResponseChangeActionPlanModel responseObj)
        {
            RestRequest request = await PrepareQuery("Responses/ChangeAction", Method.PUT);
            request.AddParameter("application/json", JsonConvert.SerializeObject(responseObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<ResponseModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"AddActionToResponse: Error while adding action to response: {queryResult.StatusCode}");
            return null;
        }

        public async Task<bool> SetSafeStatus(ResponseSafeUpdateModel responseSafeUpdateObj)
        {
            RestRequest request = await PrepareQuery("Responses/Sage", Method.PUT);
            request.AddParameter("application/json", JsonConvert.SerializeObject(responseSafeUpdateObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<bool>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"SetSafeStatus: Error while adding action to response: {queryResult.StatusCode}");
            return false;
        }
    }
}
