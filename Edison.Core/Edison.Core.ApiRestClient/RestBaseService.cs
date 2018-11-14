using Edison.Core.Config;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using RestSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Edison.Core
{
    public class RestServiceBase
    {
        private readonly string _clientId;
        private readonly AuthenticationContext _authContext = null;
        private readonly ClientCredential _clientCredential = null;
        protected readonly ILogger<RestServiceBase> _logger;
        protected readonly RestClient _client;
        private Func<string> _getTokenFunction;
        private Func<Task<string>> _getTokenFunctionAsync;

        public RestServiceBase(AzureAdOptions azureConfig, string restServiceUrl, ILogger<RestServiceBase> logger)
        {
            _logger = logger;
            _clientId = azureConfig.ClientId;
            _authContext = new AuthenticationContext(azureConfig.Instance + azureConfig.TenantId);
            _clientCredential = new ClientCredential(azureConfig.ClientId, azureConfig.ClientSecret);
            _client = new RestClient(restServiceUrl);
            _getTokenFunctionAsync = GetAzureAdToken;
        }

        public RestServiceBase(string instance, string tenantId, string clientId, string clientSecret, string restServiceUrl, ILogger<RestServiceBase> logger)
        {
            _logger = logger;
            _clientId = clientId;
            _authContext = new AuthenticationContext(instance + tenantId);
            _clientCredential = new ClientCredential(clientId, clientSecret);
            _client = new RestClient(restServiceUrl);
            _getTokenFunctionAsync = GetAzureAdToken;
        }

        public RestServiceBase(string restServiceUrl, ILogger<RestServiceBase> logger)
        {
            _logger = logger;
            _client = new RestClient(restServiceUrl);
        }

        private async Task<string> GetAzureAdToken()
        {
            AuthenticationResult result = null;
            int retryCount = 0;
            bool retry = false;

            do
            {
                retry = false;
                try
                {
                    // ADAL includes an in memory cache, so this call will only send a message to the server if the cached token is expired.
                    result = await _authContext.AcquireTokenAsync(_clientId, _clientCredential);
                }
                catch (AdalException ex)
                {
                    if (ex.ErrorCode == "temporarily_unavailable")
                    {
                        retry = true;
                        retryCount++;
                        Thread.Sleep(3000);
                    }
                    _logger?.LogError($"An error occurred while acquiring a token: {ex.Message}. Retry: {retry.ToString()}");
                }

            } while ((retry == true) && (retryCount < 3));

            return result?.AccessToken;
        }

        protected void SetTokenRetrievalFunction(Func<string> tokenFunction)
        {
            _getTokenFunction = tokenFunction;
        }

        protected void SetTokenRetrievalFunction(Func<Task<string>> tokenFunction)
        {
            _getTokenFunctionAsync = tokenFunction;
        }

        protected async Task<RestRequest> PrepareQuery(string endpoint, Method method)
        {
            RestRequest request = new RestRequest(endpoint, method);

            //Generate token
            string token = null;
            if (_getTokenFunctionAsync != null)
                token = await _getTokenFunctionAsync();
            if (_getTokenFunction != null)
                token = _getTokenFunction();
            if(!string.IsNullOrEmpty(token))
                request.AddHeader("Authorization", $"Bearer {token}");

            return request;
        }
    }
}
