using Edison.Core.Config;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using RestSharp;
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

        public RestServiceBase(AzureAdOptions azureConfig, string restServiceUrl, ILogger<RestServiceBase> logger)
        {
            _logger = logger;
            _clientId = azureConfig.ClientId;
            _authContext = new AuthenticationContext(azureConfig.Instance + azureConfig.TenantId);
            _clientCredential = new ClientCredential(azureConfig.ClientId, azureConfig.ClientSecret);
            _client = new RestClient(restServiceUrl);
        }

        public RestServiceBase(string authority, string tenantId, string clientId, string clientSecret, string restServiceUrl, ILogger<RestServiceBase> logger)
        {
            _logger = logger;
            _clientId = authority;
            _authContext = new AuthenticationContext(authority + tenantId);
            _clientCredential = new ClientCredential(clientId, clientSecret);
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

        protected async Task<RestRequest> PrepareQuery(string endpoint, Method method)
        {
            var token = await GetAzureAdToken();

            RestRequest request = new RestRequest(endpoint, method);
            request.AddHeader("Authorization", $"Bearer {token}");

            return request;
        }
    }
}
