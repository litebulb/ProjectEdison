using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace Edison.Mobile.Common.Network
{
    public class ResponseRestService : BaseRestService
    {
        public ResponseRestService(AuthService authService, ILogger logger, string baseUrl) : base(authService, logger, baseUrl) { }

        public async Task<IEnumerable<ResponseLightModel>> GetResponses()
        {

            return new List<ResponseLightModel>
            {
                new ResponseLightModel
                {
                    ResponseId = new Guid(),
                },
                new ResponseLightModel
                {
                    ResponseId = new Guid(),
                },
            };

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
            var mock = new ResponseModel
            {
                ActionPlan = new ActionPlanModel
                {
                    Color = "red",
                    CreationDate = new DateTime(2018, 9, 20, 5, 59, 04),
                    UpdateDate = new DateTime(2018, 9, 20, 6, 03, 45),
                    Name = "Fire",
                    Description = "Pre-configured action plan for a fire",
                    Icon = "fire",
                    IsActive = true,
                },
                Geolocation = new Edison.Core.Common.Models.Geolocation
                {
                    Latitude = 41.405372,
                    Longitude = 2.157819,
                },
            };

            return mock;

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
    }
}
