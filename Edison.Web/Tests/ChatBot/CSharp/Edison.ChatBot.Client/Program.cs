using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using Edison.Core.Interfaces;
using Edison.Core;

namespace Edison.ChatBot.Client
{
    class Program
    {
        private static Application _App;

        static void Main(string[] args)
        {
            //DI
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            _App = serviceProvider.GetService<Application>();

            _App.StartBotConversation().Wait();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            ILoggerFactory loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug();

            services.AddSingleton(loggerFactory);
            services.AddLogging();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            services.AddSingleton(configuration);

            // Support typed Options
            services.AddOptions();
            services.Configure<ClientConfig>(configuration.GetSection("Client"));
            services.AddSingleton<Application>();
        }

        
    }
}
