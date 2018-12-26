using System;
using System.Threading.Tasks;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Network;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace Edison.Mobile.Admin.Client.Core.Network
{
    public class OnboardingRestService : BaseRestService
    {
        public OnboardingRestService(AuthService authService, ILogger logger, string baseUrl)
            : base(authService, logger, baseUrl)
        {
            client.Authenticator = new HttpBasicAuthenticator("Administrator", "Edison1234");
        }

        protected override void AddAuthHeader(RestRequest request)
        {
            // no-op
        }

        public async Task GetDeviceId()
        {
            try
            {
                var request = PrepareRequest("/GetDeviceId", Method.GET);
                var result = await client.ExecuteTaskAsync<object>(request); // currently times out
                Console.WriteLine(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
