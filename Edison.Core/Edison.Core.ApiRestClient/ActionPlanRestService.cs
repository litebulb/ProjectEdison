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
    public class ActionPlanRestService : RestServiceBase, IActionPlanRestService
    {
        public ActionPlanRestService(IOptions<RestServiceOptions> config, ILogger<ResponseRestService> logger)
            : base(config.Value.AzureAd, config.Value.RestServiceUrl, logger)
        {
        }

        public ActionPlanRestService(string authority, string tenantId, string clientId, string clientSecret, string restServiceUrl, ILogger<RestServiceBase> logger)
            : base(authority, tenantId, clientId, clientSecret, restServiceUrl, logger)
        {
        }

        public async Task<ActionPlanModel> GetActionPlan(Guid actionPlanId)
        {
            RestRequest request = await PrepareQuery("ActionPlans/{actionPlanId}", Method.GET);
            request.AddUrlSegment("actionPlanId", actionPlanId);
            var queryResult = await _client.ExecuteTaskAsync<ActionPlanModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetActionPlan: {queryResult.StatusCode}");
            return null;
        }

        public async Task<IEnumerable<ActionPlanListModel>> GetActionPlans()
        {
            RestRequest request = await PrepareQuery("ActionPlans", Method.GET);
            var queryResult = await _client.ExecuteTaskAsync<IEnumerable<ActionPlanListModel>>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetActionPlans: {queryResult.StatusCode}");
            return null;
        }

        public async Task<ActionPlanModel> UpdateActionPlan(ActionPlanUpdateModel actionPlanObj)
        {
            RestRequest request = await PrepareQuery("ActionPlans", Method.PUT);
            request.AddParameter("application/json", JsonConvert.SerializeObject(actionPlanObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<ActionPlanModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"UpdateActionPlan: Error while updating an actionplan: {queryResult.StatusCode}");
            return null;
        }

        public async Task<ActionPlanModel> CreateActionPlan(ActionPlanCreationModel actionPlanObj)
        {
            RestRequest request = await PrepareQuery("ActionPlans", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(actionPlanObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<ActionPlanModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"CreationModel: Error while adding a device: {queryResult.StatusCode}");
            return null;
        }

        public async Task<bool> DeleteActionPlan(Guid actionPlanId)
        {
            RestRequest request = await PrepareQuery("ActionPlans", Method.DELETE);
            request.AddParameter("actionPlanId", actionPlanId.ToString());
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
