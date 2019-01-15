using System;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.Models;
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

        public async Task<ResultCommandGetDeviceId> GetDeviceId()
        {
            try
            {
                var request = PrepareRequest("/GetDeviceId", Method.GET);
                var result = await client.ExecuteGetTaskAsync<ResultCommandGetDeviceId>(request);
                if (result.IsSuccessful)
                {
                    return result.Data;
                }

                return null;
            }
            catch (Exception e)
            {
                logger.Log(e);
                return null;
            }
        }

        public async Task<ResultCommandGenerateCSR> GetGeneratedCSR()
        {
            try
            {
                var request = PrepareRequest("/GetGeneratedCSR", Method.GET);
                var queryResult = await client.ExecuteGetTaskAsync<ResultCommandGenerateCSR>(request);
                if (queryResult.IsSuccessful)
                {
                    return queryResult.Data;
                }

                return null;
            }
            catch (Exception e)
            {
                logger.Log(e);
                return null;
            }
        }

        public async Task<ResultCommand> SetDeviceType(RequestCommandSetDeviceType setDeviceTypeCommand)
        {
            try
            {
                var request = PrepareRequest("/SetDeviceType", Method.POST, setDeviceTypeCommand);
                var queryResult = await client.ExecutePostTaskAsync<ResultCommand>(request);
                if (queryResult.IsSuccessful)
                {
                    return queryResult.Data;
                }

                return null;
            }
            catch (Exception e)
            {
                logger.Log(e);
                return null;
            }
        }
    }
}
