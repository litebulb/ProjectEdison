using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Network;
using RestSharp;

namespace Edison.Mobile.User.Client.Core.Network
{
    public class ActionPlanRestService : BaseRestService
    {
        public ActionPlanRestService(AuthService authService, ILogger logger, string baseUrl)
            : base(authService, logger, baseUrl)
        {
        }

        public async Task<ActionPlanModel> GetActionPlan(Guid actionPlanId)
        {
            var request = PrepareRequest("ActionPlans/{actionPlanId}", Method.GET);
            request.AddUrlSegment("actionPlanId", actionPlanId);

            var queryResult = await client.ExecuteTaskAsync<ActionPlanModel>(request);
            if (queryResult.IsSuccessful)
            {
                return queryResult.Data;
            }

            logger.Log($"Error getting action plan. Response Status: {queryResult.ResponseStatus}, Error Message: {queryResult.ErrorMessage}");

            return null;
        }

        public async Task<IEnumerable<ActionPlanListModel>> GetActionPlans()
        {
            var request = PrepareRequest("ActionPlans", Method.GET);
            var queryResult = await client.ExecuteTaskAsync<IEnumerable<ActionPlanListModel>>(request);

            if (queryResult.IsSuccessful)
            {
                return queryResult.Data;
            }

            logger.Log("Error getting action plans. Response Status: {queryResult.ResponseStatus}, Error Message: {queryResult.ErrorMessage}");

            return null;
        }
    }
}
