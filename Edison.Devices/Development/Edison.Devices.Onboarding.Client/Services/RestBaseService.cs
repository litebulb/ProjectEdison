using RestSharp;
using System;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Client.Services
{
    public class RestBaseService
    {
        private readonly string _token;
        protected readonly RestClient _client;
        private readonly string _tokenType;

        public RestBaseService(string restServiceUrl, string token)
        {
            _token = token;
            _client = new RestClient(restServiceUrl);
            _tokenType = "Bearer";
        }

        public RestBaseService(string restServiceUrl, string token, string tokenType)
        {
            _token = token;
            _client = new RestClient(restServiceUrl);
            _tokenType = tokenType;
        }

#pragma warning disable 1998
        protected async Task<RestRequest> PrepareQuery(string endpoint, Method method)
        {
            RestRequest request = new RestRequest(endpoint, method);
            request.AddHeader("Authorization", $"{_tokenType} {_token}");

            return request;
        }
    }
}
