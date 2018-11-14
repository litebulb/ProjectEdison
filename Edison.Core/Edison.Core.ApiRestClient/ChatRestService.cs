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
    public class ChatRestService : RestServiceBase, IChatRestService
    {
        public ChatRestService(IOptions<RestServiceOptions> config, ILogger<EventClusterRestService> logger)
            : base(config.Value.AzureAd, config.Value.RestServiceUrl, logger)
        {
        }

        public ChatRestService(string authority, string tenantId, string clientId, string clientSecret, string restServiceUrl, ILogger<RestServiceBase> logger)
            : base(authority, tenantId, clientId, clientSecret, restServiceUrl, logger)
        {
        }

        public async Task<ChatUserTokenContext> GetToken()
        {
            RestRequest request = await PrepareQuery("Security/GetToken", Method.GET);
            var queryResult = await _client.ExecuteTaskAsync<ChatUserTokenContext>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetToken: {queryResult.StatusCode}");
            return null;
        }
    }
}
