using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace Edison.Api
{
    /// <summary>
    /// Middleware that handles any errors happening on API endpoints
    /// </summary>
    internal class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;

        }

        public async Task Invoke(HttpContext context, ILogger<ErrorHandlingMiddleware> logger)
        {
            try
            {
                await next(context);
            }
            catch (UnauthorizedAccessException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Failed Access Authorization");
            }
            catch (DocumentClientException dCE)
            {
                await OnDocumentClientExceptionAsync(context, dCE, logger);
            }
            catch (HttpRequestException hre)
            {
                await OnHttpRequestExceptionAsync(context, hre, logger);
            }
            catch (Exception ex)
            {
                await OnExceptionAsync(context, ex, logger);
            }
        }

        public async Task OnDocumentClientExceptionAsync(HttpContext context, DocumentClientException ex, ILogger<ErrorHandlingMiddleware> logger)
        {
            if (ex.StatusCode != null && ex.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
            {
                string message = CreateErrorContent("An update failed because it was previously modified",
                "PreconditionFailed");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status412PreconditionFailed;
                await context.Response.WriteAsync(message);
            }
            else
                await OnExceptionAsync(context, ex, logger);
        }

        public async Task OnExceptionAsync(HttpContext context, Exception ex, ILogger<ErrorHandlingMiddleware> logger)
        {
            string message = CreateErrorContent("An Unexpected Error Occurred, Please Contact Your Administrator.",
                "UnexpectedError");

            logger.LogError(ex.Message);
            logger.LogError(ex.StackTrace);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;


            await context.Response.WriteAsync(message);
        }

        public async Task OnHttpRequestExceptionAsync(HttpContext context, HttpRequestException ex, ILogger<ErrorHandlingMiddleware> logger)
        {
            string message = CreateErrorContent("An Unexpected Error Occurred, Please Contact Your Administrator.",
                "NotFound");

            logger.LogError(ex.Message);
            logger.LogError(ex.StackTrace);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status404NotFound;


            await context.Response.WriteAsync(message);
        }


        private string CreateErrorContent(string message, string errorCode)
        {
            var errorMessage = new
            {
                Message = message,
                ErrorCode = errorCode
            };

            return JsonConvert.SerializeObject(errorMessage);

        }
    }
}
