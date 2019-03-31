using System;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Deserializers;

namespace Edison.Mobile.Common.Network
{
    public class BaseRestService
    {
        protected readonly AuthService authService;
        protected readonly ApiClient client;
        protected readonly ILogger logger;

        public BaseRestService(AuthService authService, ILogger logger, string baseUrl)
        {
            this.logger = logger;
            this.authService = authService;

            client = new ApiClient(new RestClient(baseUrl));
        }

        protected virtual RestRequest PrepareRequest(string endpoint, Method method, object requestBody = null)
        {
            try
            {
                var request = new RestRequest(endpoint, method) { RequestFormat = DataFormat.Json };

                AddAuthHeader(request);

                if ((method == Method.POST || method == Method.PUT) && requestBody != null)
                {
                    request.AddParameter("application/json", JsonConvert.SerializeObject(requestBody), ParameterType.RequestBody);
                }

                return request;
            }
            catch (Exception ex)
            {
                logger.Log(ex, "Error preparing REST request");
                return new RestRequest();
            }
        }

        protected virtual void AddAuthHeader(RestRequest request)
        {
            var token = authService.AuthenticationResult.IdToken;
            request.AddHeader("Authorization", $"Bearer {token}");
        }
    }

    public class NewtonsoftDeserializer : IDeserializer
    {
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
        public string ContentType { get; set; }
            
        public T Deserialize<T>(IRestResponse response)
        {
            try
            {
                ITraceWriter traceWriter = new MemoryTraceWriter();
                var deserialized = JsonConvert.DeserializeObject<T>(response.Content, new JsonSerializerSettings { TraceWriter = traceWriter });
                Console.WriteLine(traceWriter);
                return deserialized;
            } 
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default(T);
            }
        }
    }
}
