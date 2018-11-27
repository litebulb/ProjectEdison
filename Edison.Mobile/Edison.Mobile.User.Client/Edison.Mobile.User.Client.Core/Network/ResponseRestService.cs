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
            var request = PrepareRequest("Responses", Method.GET);
            var queryResult = await client.ExecuteGetTaskAsync<IEnumerable<ResponseLightModel>>(request);
            if (queryResult.IsSuccessful)
            {
                return queryResult.Data;
            }

            logger.Log($"Error getting responses. Response Status: {queryResult.ResponseStatus}, Error Message: {queryResult.ErrorMessage}", LogLevel.Error);

            return null;
        }

        public async Task<ResponseModel> GetResponse(Guid responseId)
        {
            var request = PrepareRequest("Responses/{responseId}", Method.GET);
            request.AddUrlSegment("responseId", responseId);

            var queryResult = await client.ExecuteGetTaskAsync<ResponseModel>(request);
            if (queryResult.IsSuccessful)
            {
                return queryResult.Data;
            }

            logger.Log($"GetResponse: {queryResult.ResponseStatus}, {queryResult.ErrorMessage}", LogLevel.Error);

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

            logger.Log($"Error getting response notifications. Response Status: {queryResult.ResponseStatus}, Error Message: {queryResult.ErrorMessage}", LogLevel.Error);

            return null;
        }

        public async Task<bool> SendIsSafe(bool isSafe) 
        {
            var request = PrepareRequest("Responses/Safe", Method.PUT, new 
            {
                IsSafe = isSafe,
            });

            var queryResult = await client.ExecuteTaskAsync(request);
            if (!queryResult.IsSuccessful) 
            {
                logger.Log($"Error sending is safe, {queryResult.ErrorMessage}", LogLevel.Error);
            }

            return queryResult.IsSuccessful;
        }
    }
}
