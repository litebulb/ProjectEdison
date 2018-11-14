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
    public class ResponseRestService : BaseRestService
    {
        public ResponseRestService(AuthService authService, ILogger logger, string baseUrl) : base(authService, logger, baseUrl) { }

        public async Task<IEnumerable<ResponseLightModel>> GetResponses()
        {
            //return new List<ResponseLightModel>
            //{
            //    new ResponseLightModel
            //    {
            //        ResponseId = new Guid(),
            //    },
            //    new ResponseLightModel
            //    {
            //        ResponseId = new Guid(),
            //    },
            //};

            var request = PrepareRequest("Responses", Method.GET);
            var queryResult = await client.ExecuteGetTaskAsync<IEnumerable<ResponseLightModel>>(request);
            if (queryResult.IsSuccessful)
            {
                return queryResult.Data;
            }

            logger.Log($"Error getting responses. Response Status: {queryResult.ResponseStatus}, Error Message: {queryResult.ErrorMessage}");

            return null;
        }

        public async Task<ResponseModel> GetResponse(Guid responseId)
        {
            //return new ResponseModel
            //{
            //    ActionPlan = new ResponseActionPlanModel
            //    {
            //        Color = "red",
            //        Name = "Fire",
            //        Description = "Pre-configured action plan for a fire",
            //        Icon = "fire",
            //    },
            //    Geolocation = new Edison.Core.Common.Models.Geolocation
            //    {
            //        Latitude = 41.405372,
            //        Longitude = 2.157819,
            //    },
            //};

            var request = PrepareRequest("Responses/{responseId}", Method.GET);
            request.AddUrlSegment("responseId", responseId);

            var queryResult = await client.ExecuteGetTaskAsync<ResponseModel>(request);
            if (queryResult.IsSuccessful)
            {
                return queryResult.Data;
            }

            logger.Log($"GetResponse: {queryResult.ResponseStatus}, {queryResult.ErrorMessage}");

            return null;
        }

        public async Task<IEnumerable<NotificationModel>> GetNotifications(string responseId)
        {
            var request = PrepareRequest("Notifications/Responses/{responseId}", Method.GET);
            request.AddUrlSegment("responseId", responseId);

            var queryResult = await client.ExecuteGetTaskAsync<IEnumerable<NotificationModel>>(request);

            if (queryResult.IsSuccessful)
            {
                return queryResult.Data;
            }

            logger.Log($"Error getting response notifications. Response Status: {queryResult.ResponseStatus}, Error Message: {queryResult.ErrorMessage}");

            return null;
        }
    }
}
