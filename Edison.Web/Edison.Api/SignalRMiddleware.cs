using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Edison.Api
{
    /// <summary>
    /// Middleware to handle signalr authentication.
    /// The token has to be passed in the query for signalr. The middleware ensure that the parameter 
    /// passed in query is translated into a Authorize header
    /// </summary>
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
