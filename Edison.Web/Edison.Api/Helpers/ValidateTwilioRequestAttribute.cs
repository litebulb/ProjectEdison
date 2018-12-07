using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Edison.Api.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Twilio.Security;

namespace Edison.Api.Helpers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateTwilioRequestAttribute : ActionFilterAttribute
    {
        private readonly RequestValidator _requestValidator;

        public ValidateTwilioRequestAttribute(IOptions<TwilioOptions> options)
        {
            var authToken = options.Value.AuthToken;
            _requestValidator = new RequestValidator(authToken);
        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var context = actionContext.HttpContext;
            if (!IsValidRequest(context.Request))
            {
                actionContext.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            }

            base.OnActionExecuting(actionContext);
        }

        private bool IsValidRequest(HttpRequest request)
        {
            var requestUrl = RequestRawUrl(request);
            var parameters = ToDictionary(request.Form);
            var signature = request.Headers["X-Twilio-Signature"];
            return _requestValidator.Validate(requestUrl, parameters, signature);
        }

        private static string RequestRawUrl(HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        }

        private static IDictionary<string, string> ToDictionary(IFormCollection collection)
        {
            return collection.Keys
                .Select(key => new { Key = key, Value = collection[key] })
                .ToDictionary(p => p.Key, p => p.Value.ToString());
        }
    }
}
