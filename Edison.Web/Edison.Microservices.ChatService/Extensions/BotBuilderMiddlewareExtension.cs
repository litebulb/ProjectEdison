using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Bot.Builder.Integration.AspNet.Core
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBotMiddleware<T>(this IApplicationBuilder applicationBuilder) where T : IMiddleware
        {
            var botFrameworkOptions = applicationBuilder.ApplicationServices.GetService<IOptions<BotFrameworkOptions>>();
            botFrameworkOptions.Value.Middleware.Add(applicationBuilder.ApplicationServices.GetService<T>());
            return applicationBuilder;
        }
    }
}
