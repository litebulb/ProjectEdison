using Microsoft.IdentityModel.Clients.ActiveDirectory;
using RestSharp;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Client.Services
{
    public class RestServiceBase
    {
        private readonly string _token;
        protected readonly RestClient _client;

        public RestServiceBase(string restServiceUrl, string token)
        {
            _token = token;
            _client = new RestClient(restServiceUrl);
        }
        protected RestRequest PrepareQuery(string endpoint, Method method)
        {
            RestRequest request = new RestRequest(endpoint, method);
            request.AddHeader("Authorization", $"Bearer {_token}");

            return request;
        }
    }
}
