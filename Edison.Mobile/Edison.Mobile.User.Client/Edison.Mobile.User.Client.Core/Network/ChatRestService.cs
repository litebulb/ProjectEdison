using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Network;
using RestSharp;

namespace Edison.Mobile.User.Client.Core.Network
{
    public class ChatRestService : BaseRestService
    {
        public ChatRestService(AuthService authService, ILogger logger, string baseUrl) : base(authService, logger, baseUrl)
        {
        }

        public async Task<ChatUserTokenContext> GetToken() 
        {
            var request = PrepareRequest("Security/GetToken", Method.GET);
            var queryResult = await client.ExecuteGetTaskAsync<ChatUserTokenContext>(request);

            if (queryResult.IsSuccessful)
            {
                return queryResult.Data;
            }

            logger.Log($"Error getting chat token. Response Status: {queryResult.ResponseStatus}, Error Message: {queryResult.ErrorMessage}");

            return null;
        }
    }
}
