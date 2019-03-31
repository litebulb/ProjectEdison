using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Network;
using Edison.Mobile.Common.WiFi;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Serialization;

namespace Edison.Mobile.Common.Network
{
    public class ApiClient 
    {
        private readonly IRestClient wrapped;

        public IAuthenticator Authenticator
        {
            get { return wrapped.Authenticator; }
            set { wrapped.Authenticator = value; }
        }

        public ApiClient(IRestClient wrapped)
        {
            this.wrapped = wrapped;
            this.wrapped.AddHandler("application/json", new NewtonsoftDeserializer());
        }

        private async Task<IRestResponse<T>> Retry<T>(IRestRequest request, Func<IRestRequest, Task<IRestResponse<T>>> delegated)
        {
            IRestResponse<T> response = default(IRestResponse<T>);

            int tryCount = 0;

            do
            {
                tryCount++;
                response = await delegated(request);
            }
            while (!response.IsSuccessful && tryCount < 5);

            return response;
        }


        public async Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request)
        {
            IRestResponse<T> response = default(IRestResponse<T>);

            int tryCount = 0;

            do
            {
                tryCount++;
                response = await this.wrapped.ExecuteGetTaskAsync<T>(request);
            }
            while (!response.IsSuccessful && tryCount < 5);

            return response;
        }

        public async Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request)
        {
            IRestResponse<T> response = default(IRestResponse<T>);

            int tryCount = 0;

            do
            {
                tryCount++;
                response = await this.wrapped.ExecuteTaskAsync<T>(request);
            }
            while (!response.IsSuccessful && tryCount < 5);

            return response;
            //return await Retry<T>(request, r => this.wrapped.ExecuteTaskAsync<T>(r));            
        }

        public async Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request)
        {
            IRestResponse<T> response = default(IRestResponse<T>);

            int tryCount = 0;

            do
            {
                tryCount++;
                response = await this.wrapped.ExecuteTaskAsync<T>(request);
            }
            while (!response.IsSuccessful && tryCount < 5);

            return response;
            //return await Retry<T>(request, r => this.wrapped.ExecutePostTaskAsync<T>(r));            
        }
    }
}
