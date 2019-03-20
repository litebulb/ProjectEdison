using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.Models;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Network;
using Edison.Mobile.Common.WiFi;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace Edison.Mobile.Admin.Client.Core.Network
{
    public class OnboardingRestService : BaseRestService, IOnboardingRestService
    {
        public OnboardingRestService(AuthService authService, ILogger logger, string baseUrl)
            : base(authService, logger, baseUrl)
        {
        }

        protected override void AddAuthHeader(RestRequest request)
        {
            // no-op
        }

        public void SetBasicAuthentication(string password)
        {
            client.Authenticator = new HttpBasicAuthenticator("Administrator", password);
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

        public async Task<ResultCommandGetNetworkProfiles> GetNetworkProfiles()
        {
            try
            {
                var request = PrepareRequest("/GetNetworkProfiles", Method.GET);
                var result = await client.ExecuteGetTaskAsync<ResultCommandGetNetworkProfiles>(request);
                if (result.IsSuccessful)
                {
                    return result.Data;
                }

            }
            catch (Exception e)
            {
                logger.Log(e);
            }

            return default(ResultCommandGetNetworkProfiles);
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

        public async Task<ResultCommand> ProvisionDevice(RequestCommandProvisionDevice provisionDeviceCommand)
        {
            try
            {
                var request = PrepareRequest("/ProvisionDevice", Method.POST, provisionDeviceCommand);
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

        public async Task<IEnumerable<AvailableNetwork>> GetAvailableWifiNetworks()
        {
            try
            {
                var request = PrepareRequest("/GetAvailableNetworks", Method.GET);
                var queryResult = await client.ExecuteTaskAsync<ResultCommandAvailableNetworks>(request);
                if (queryResult.IsSuccessful)
                {
                    return queryResult.Data.Networks;
                }

                return null;
            }
            catch (Exception e)
            {
                logger.Log(e);
                return null;
            }
        }

        public async Task<ResultCommandNetworkStatus> ConnectToNetwork(RequestNetworkInformationModel networkInformationModel)
        {
            try
            {
                var request = PrepareRequest("/ConnectToNetwork", Method.POST, networkInformationModel);
                var queryResult = await client.ExecutePostTaskAsync<ResultCommandNetworkStatus>(request);
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

        public async Task<ResultCommand> SetDeviceSecretKeys(RequestCommandSetDeviceSecretKeys command)
        {
            try
            {
                var request = PrepareRequest("/SetDeviceSecretKeys", Method.POST, command);
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
