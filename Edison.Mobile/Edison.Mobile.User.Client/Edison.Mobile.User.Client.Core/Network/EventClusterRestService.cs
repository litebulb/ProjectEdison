using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Network;
using RestSharp;

namespace Edison.Mobile.User.Client.Core.Network
{
    public class EventClusterRestService : BaseRestService
    {
        public EventClusterRestService(AuthService authService, ILogger logger, string baseUrl) : base(authService, logger, baseUrl) { }

        public async Task<EventClusterModel> GetEventCluster(string eventClusterId)
        {
            var request = PrepareRequest("EventClusters/{eventClusterId}", Method.GET);
            request.AddUrlSegment("eventClusterId", eventClusterId);

            var queryResult = await client.ExecuteGetTaskAsync<EventClusterModel>(request);

            if (queryResult.IsSuccessful) 
            {
                return queryResult.Data;
            }

            logger.Log($"Error getting event cluster. Response Status: {queryResult.ResponseStatus}, Error Message: {queryResult.ErrorMessage}");

            return null;
        }
    }
}
