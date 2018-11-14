using Edison.ChatService.Helpers;
using Edison.ChatService.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Edison.ChatService
{
    public partial class Startup
    {
        public void AddBot(IServiceCollection services)
        {
            //Middlewares
            services.AddSingleton<ContextMiddleware>();
            services.AddSingleton<LogMessageMiddleware>();
            services.AddSingleton<CommandMiddleware>();
            services.AddSingleton<HandoffMiddleware>();

            services.AddBot<EdisonBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration.GetSection("Bot"));
                options.Paths.BasePath = Configuration["Bot:BotBasePath"];
                options.Paths.MessagesPath = Configuration["Bot:BotMessagesPath"];
                options.OnTurnError = async (turnContext, exception) =>
                {
                    if (turnContext.TurnState.TryGetValue(typeof(MessageRouter).FullName, out object messageRouter))
                    {
                        ConversationReference selfConversation = turnContext.Activity.GetConversationReference();
                        await ((MessageRouter)messageRouter).SendErrorMessageAsync
                        (selfConversation, $"Sorry, it looks like something went wrong: {exception.Message}");
                    }
                    else
                    {
                        _logger.LogError($"Sorry, it looks like something went wrong: {exception.Message}");
                    }
                };
            });
        }

        public void ConfigureBot(IApplicationBuilder app)
        {
            app.UseBotFramework();
            app.UseBotMiddleware<ContextMiddleware>();
            app.UseBotMiddleware<CommandMiddleware>();
            app.UseBotMiddleware<LogMessageMiddleware>();
            app.UseBotMiddleware<HandoffMiddleware>();
        }
    }
}
