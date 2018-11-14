using Edison.Core;
using Edison.Core.Common.Models;
using Edison.Core.Config;
using Edison.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;

namespace Edison.ChatService.Helpers
{
    public class DirectLineRestService : RestServiceBase, IDirectLineRestService
    {
        public DirectLineRestService(string restServiceUrl, string secretToken, ILogger<RestServiceBase> logger)
            : base(restServiceUrl, logger)
        {
            SetTokenRetrievalFunction(() => { return secretToken; });
        }

        public async Task<TokenConversationResult> GenerateToken(TokenConversationParameters tokenParameters)
        {
            RestRequest request = await PrepareQuery("tokens/generate", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(tokenParameters), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<TokenConversationResult>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GenerateToken: Error while requesting a direct line token: {queryResult.StatusCode}");
            return null;
        }
    }
}
