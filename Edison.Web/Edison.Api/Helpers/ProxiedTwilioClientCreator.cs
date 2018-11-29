using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Options;
using Twilio.Clients;
using Edison.Api.Config;

namespace Edison.Api.Helpers
{
    /// <summary>
    /// Manager for Twilio service
    /// </summary>
    public class ProxiedTwilioClientCreator
    {
        private readonly IOptions<TwilioOptions> _twilioOptions;
        private static HttpClient _httpClient;

        public ProxiedTwilioClientCreator(IOptions<TwilioOptions> twilioOptions)
        {
            _twilioOptions = twilioOptions;
        }

        private void CreateHttpClient()
        {
            var proxyUrl = _twilioOptions.Value.HttpClient;
            var handler = new HttpClientHandler()
            {
                Proxy = new WebProxy(proxyUrl),
                UseProxy = true
            };

            _httpClient = new HttpClient(handler);
            var byteArray = Encoding.Unicode.GetBytes(
                _twilioOptions.Value.UserName + ":" +
                _twilioOptions.Value.Password
            );

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(byteArray));
        }

        public TwilioRestClient GetClient()
        {
            var accountSid = _twilioOptions.Value.AccountSID;
            var authToken = _twilioOptions.Value.AuthToken;

            if (_httpClient == null)
            {
                CreateHttpClient();
            }

            var twilioRestClient = new TwilioRestClient(
                accountSid,
                authToken,
                httpClient: new Twilio.Http.SystemNetHttpClient(_httpClient)
            );

            return twilioRestClient;
        }
    }
}
