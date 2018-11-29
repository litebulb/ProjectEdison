using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Edison.Core.Common;

namespace Microsoft.AspNetCore.Authentication
{
    public static class AzureAdServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddAzureAdAndB2CBearer(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            builder.AddJwtBearer(AuthenticationBearers.AzureAD, options =>
            {
                options.Audience = configuration["AzureAd:ClientId"];
                options.TokenValidationParameters.ValidIssuers = new List<string>()
                {
                    $"{configuration["AzureAd:Instance"]}{configuration["AzureAd:TenantId"]}/v2.0" //For Issuer validation for MSAL v2
                };
                options.Authority = $"{configuration["AzureAd:Instance"]}{configuration["AzureAd:TenantId"]}";
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = OnMessageReceived
                };
            })
            .AddJwtBearer(AuthenticationBearers.AzureB2C, options =>
            {
                options.Audience = configuration["AzureAdB2CWeb:ClientId"];
                options.Authority = $"{configuration["AzureAdB2CWeb:Instance"]}/{configuration["AzureAdB2CWeb:Domain"]}/{configuration["AzureAdB2CWeb:SignUpSignInPolicyId"]}/v2.0";
            });
            return builder;
        }

        public static Task OnMessageReceived(MessageReceivedContext context)
        {
            string scheme = context.Scheme.Name;
            string token = string.Empty;
            // get the scheme from the header
            string header = (string)context.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(header))
            {
                var queryAuth = context.Request.Query["Authorization"];

                if (queryAuth.Count > 0)
                {
                    header = queryAuth[0];
                }
            }

            if (string.IsNullOrEmpty(header))
            {
                context.Fail(new ApplicationException("No header present"));
                return Task.FromResult(true);
            }
            if (header.StartsWith($"{scheme} ", StringComparison.OrdinalIgnoreCase))
            {
                token = header.Substring($"{scheme} ".Length).Trim();
            }
            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(true);
            }

            // we got a token for the scheme, update so the rest of the request will process it
            // Update so the middleware can process
            context.Request.Headers["Authorization"] = $"Bearer {token}";

            return Task.FromResult(true);
        }
    }
}
