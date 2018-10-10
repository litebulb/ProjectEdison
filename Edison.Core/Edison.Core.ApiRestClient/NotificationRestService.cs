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
    public class NotificationRestService : RestServiceBase, INotificationRestService
    {
        public NotificationRestService(IOptions<RestServiceOptions> config, ILogger<ResponseRestService> logger)
            : base(config.Value.AzureAd, config.Value.RestServiceUrl, logger)
        {
        }

        public NotificationRestService(string authority, string tenantId, string clientId, string clientSecret, string restServiceUrl, ILogger<RestServiceBase> logger)
            : base(authority, tenantId, clientId, clientSecret, restServiceUrl, logger)
        {
        }
        public async Task<bool> RegisterDevice(MobileDeviceNotificationHubInstallationModel deviceInstallation)
        {
            RestRequest request = await PrepareQuery("Notifications/Register", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(deviceInstallation), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<bool>(request);
            if (!queryResult.IsSuccessful)
            {
                _logger.LogError($"RegisterDevice: Error: {queryResult.StatusCode}");
                return false;
            }
            return true;
        }
        public async Task<NotificationModel> SendNotification(NotificationModel notificationReq)
        {
            RestRequest request = await PrepareQuery("Notifications/SendNotification", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(notificationReq), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<NotificationModel>(request);
            if (queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"SendNotification: Error: {queryResult.StatusCode}");
            return null;
        }
        public async Task<IEnumerable<NotificationModel>> GetNotificationsHistory(int pageSize, string continuationToken)
        {
            RestRequest request = await PrepareQuery("Notifications?continuationToken={continuationToken}&pageSize={pageSize}", Method.GET);
            request.AddUrlSegment("pageSize", pageSize);
            request.AddUrlSegment("continuationToken", continuationToken);
            var queryResult = await _client.ExecuteTaskAsync<IEnumerable<NotificationModel>>(request);
            if (!queryResult.IsSuccessful)
                return queryResult.Data;
            else
                _logger.LogError($"GetNotificationsHistory: {queryResult.StatusCode}");
            return null;
        }
    }
}
