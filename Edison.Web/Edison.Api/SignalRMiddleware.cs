using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Edison.Api
{
    internal class SignalRMiddleware
    {
        private readonly RequestDelegate next;

        public SignalRMiddleware(RequestDelegate next)
        {
            this.next = next;

        }

        public async Task Invoke(HttpContext context, ILogger<ErrorHandlingMiddleware> logger)
        {
            if (context.Request.Query.ContainsKey("access_token"))
            {
                context.Request.Headers.Add("Authorization", $"Bearer {context.Request.Query["access_token"]}");
            }
            await next(context);
        }
    }
}
