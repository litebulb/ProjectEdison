using System;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using RestSharp;

namespace Edison.Mobile.Common.Network
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
    }
}
