using System;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using RestSharp;
using Edison.Mobile.Common.Network;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;

namespace Edison.Mobile.Common.Network
{
    public class LocationRestService : BaseRestService
    {
        public LocationRestService(AuthService authService, ILogger logger, string baseUrl) : base(authService, logger, baseUrl)
        {
        }

        public async Task UpdateDeviceLocation(Geolocation geolocation) 
        {
            var request = PrepareRequest("Devices/DeviceLocation", Method.PUT, new 
            {
                Geolocation = geolocation,
            });

            var queryResult = await client.ExecuteTaskAsync<bool>(request);
            if (queryResult.IsSuccessful)
            {
                return;
            }

            logger.Log($"Error sending geolocation. Response Status: {queryResult.ResponseStatus}, Error Message: {queryResult.ErrorMessage}");
        }
    }
}
