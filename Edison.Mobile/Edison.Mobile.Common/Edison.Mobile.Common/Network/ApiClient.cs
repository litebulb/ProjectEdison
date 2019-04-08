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

        private async Task<IRestResponse<T>> Retry<T>(IRestRequest request, Func<IRestRequest, Task<IRestResponse<T>>> delegated, CancellationToken cancellationToken = default(CancellationToken))
        {
            IRestResponse<T> response = default(IRestResponse<T>);

            int tryCount = 0;

            do
            {
                tryCount++;

                if (tryCount > 1)
                {
                    await Task.Delay(1000 * (tryCount));
                }

                response = await delegated(request);                
            }
            while (!response.IsSuccessful && tryCount < 6 && (cancellationToken == default(CancellationToken) || !cancellationToken.IsCancellationRequested));

            return response;
        }

        private async Task<IRestResponse> Retry(IRestRequest request, Func<IRestRequest, Task<IRestResponse>> delegated, CancellationToken cancellationToken = default(CancellationToken))
        {
            IRestResponse response = default(IRestResponse);

            int tryCount = 0;

            do
            {
                tryCount++;

                if (tryCount > 1)
                {
                    await Task.Delay(1000 * (tryCount));
                }

                response = await delegated(request);
            }
            while (!response.IsSuccessful && tryCount < 6 && (cancellationToken == default(CancellationToken) || !cancellationToken.IsCancellationRequested));

            return response;
        }

        public async Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request, CancellationToken cancellationToken)
        {
            return await Retry<T>(request, r => this.wrapped.ExecuteGetTaskAsync<T>(r, cancellationToken), cancellationToken);
        }

        public async Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, CancellationToken cancellationToken)
        {
            return await Retry<T>(request, r => this.wrapped.ExecuteTaskAsync<T>(r, cancellationToken), cancellationToken);
        }

        public async Task<IRestResponse> ExecuteTaskAsync(IRestRequest request, CancellationToken cancellationToken)
        {
            return await Retry(request, r => this.wrapped.ExecuteTaskAsync(r, cancellationToken), cancellationToken);
        }

        public async Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request, CancellationToken cancellationToken)
        {
            return await Retry<T>(request, r => this.wrapped.ExecutePostTaskAsync<T>(r, cancellationToken), cancellationToken);
        }

        public async Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request)
        {
            return await Retry<T>(request, r => this.wrapped.ExecuteGetTaskAsync<T>(r));
        }

        public async Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request)
        {
            return await Retry<T>(request, r => this.wrapped.ExecuteTaskAsync<T>(r));            
        }

        public async Task<IRestResponse> ExecuteTaskAsync(IRestRequest request)
        {
            return await Retry(request, r => this.wrapped.ExecuteTaskAsync(r));
        }

        public async Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request)
        {
            return await Retry<T>(request, r => this.wrapped.ExecutePostTaskAsync<T>(r));            
        }
    }
}
