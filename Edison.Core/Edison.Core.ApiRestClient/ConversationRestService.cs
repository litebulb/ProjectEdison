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
    public class ConversationRestService : RestServiceBase, IConversationRestService
    {
        public ConversationRestService(IOptions<RestServiceOptions> config, ILogger<EventClusterRestService> logger)
            : base(config.Value.AzureAd, config.Value.RestServiceUrl, logger)
        {
        }

        public ConversationRestService(string authority, string tenantId, string clientId, string clientSecret, string restServiceUrl, ILogger<RestServiceBase> logger)
            : base(authority, tenantId, clientId, clientSecret, restServiceUrl, logger)
        {
        }

        public async Task<ReportModel> GetConversationById(Guid conversationId)
        {
            RestRequest request = await PrepareQuery("Conversations/ById/{conversationId}", Method.GET);
            request.AddUrlSegment("conversationId", conversationId);
            var queryResult = await _client.ExecuteTaskAsync<ReportModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetConversationById: {queryResult.StatusCode}");
            return null;
        }

        public async Task<ReportModel> GetActiveConversationFromUser(string userId)
        {
            RestRequest request = await PrepareQuery("Conversations/Active/{userId}", Method.GET);
            request.AddUrlSegment("userId", userId);
            var queryResult = await _client.ExecuteTaskAsync<ReportModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetActiveConversationFromUser: {queryResult.StatusCode}");
            return null;
        }

        public async Task<IEnumerable<ReportModel>> GetConversationsFromUser(string userId)
        {
            RestRequest request = await PrepareQuery("Conversations/{userId}", Method.GET);
            request.AddUrlSegment("userId", userId);
            var queryResult = await _client.ExecuteTaskAsync<IEnumerable<ReportModel>>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetConversationsFromUser: {queryResult.StatusCode}");
            return null;
        }

        public async Task<IEnumerable<ReportModel>> GetActiveConversations()
        {
            RestRequest request = await PrepareQuery("Conversations/Active", Method.GET);
            var queryResult = await _client.ExecuteTaskAsync<IEnumerable<ReportModel>>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetActiveConversations: {queryResult.StatusCode}");
            return null;
        }

        public async Task<ReportModel> CreateOrUpdateConversation(ReportLogCreationModel conversationLogObj)
        {
            RestRequest request = await PrepareQuery("Conversations", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(conversationLogObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<ReportModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"CreateOrUpdateConversation: Error while adding a conversation: {queryResult.StatusCode}");
            return null;
        }

        public async Task<ReportModel> CloseConversation(ReportLogCloseModel conversationCloseObj)
        {
            RestRequest request = await PrepareQuery("Conversations/Close", Method.PUT);
            request.AddParameter("application/json", JsonConvert.SerializeObject(conversationCloseObj), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<ReportModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"CloseConversation: Error while closing a conversation: {queryResult.StatusCode}");
            return null;
        }
    }
}
