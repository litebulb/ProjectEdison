using Edison.Core.Common.Models;
using Edison.Core.Config;
using Edison.Core.Interfaces;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edison.Core
{
    public class TwilioRestService : RestServiceBase, ITwilioRestService
    {
        public TwilioRestService(IOptions<RestServiceOptions> config, ILogger<TwilioRestService> logger)
            : base(config.Value.AzureAd, config.Value.RestServiceUrl, logger)
        {
        }

        public TwilioRestService(string authority, string tenantId, string clientId, string clientSecret, string restServiceUrl, ILogger<RestServiceBase> logger)
            : base(authority, tenantId, clientId, clientSecret, restServiceUrl, logger)
        {
        }
        public async Task<TwilioModel> EmergencyCall(TwilioModel twilioReq)
        {
            RestRequest request = await PrepareQuery("Twilio/Emergency", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(twilioReq), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<TwilioModel>(request);
            if (queryResult.IsSuccessful)
            {
                //return the Twilio call SID
                return queryResult.Data;
            }
            else
                _logger.LogError($"EmergencyCall: Error: {queryResult.StatusCode}");
            return null;
        }

        public async Task<string> Interconnect()
        {
            RestRequest request = await PrepareQuery("Twilio/Interconnect", Method.POST);
            var queryResult = await _client.ExecuteTaskAsync<string>(request);
            if (queryResult.IsSuccessful)
            {
                return queryResult.Data;
            }
            {
                _logger.LogError($"Interconnect: Error: {queryResult.StatusCode}");
            }
            return null;
        }
    }
}
