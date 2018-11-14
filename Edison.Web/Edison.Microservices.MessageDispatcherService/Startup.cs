using Edison.Common.Interfaces;
using Edison.Common.Config;
using Edison.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Edison.MessageDispatcherService.Config;

namespace Edison.MessageDispatcherService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.EnableKubernetes();
            services.AddApplicationInsightsTelemetry(Configuration);
            services.Configure<ServiceBusRabbitMQOptions>(Configuration.GetSection("ServiceBusRabbitMQ"));
            services.Configure<AzureServiceBusOptions>(Configuration.GetSection("AzureServiceBus"));
            services.Configure<MessageDispatcherOptions>(Configuration.GetSection("MessageDispatcher"));

            services.AddSingleton<IAzureServiceBusClient, AzureServiceBusClient>();
            services.AddSingleton<IMassTransitServiceBus, ServiceBusRabbitMQ>();
            services.AddSingleton<MessageDispatcherService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // mirror logger messages to AppInsights
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Debug);

            if (Configuration.GetValue<bool>("MessageDispatcher:DispatcherEnabled"))
            {
                app.ApplicationServices.GetService<IMassTransitServiceBus>().Start(null);
                app.ApplicationServices.GetService<MessageDispatcherService>().Start();

                app.Run(context =>
                {
                    return context.Response.WriteAsync("MessageDispatcherService Service is running...");
                });
            }
            else
            {
                app.Run(context =>
                {
                    return context.Response.WriteAsync("MessageDispatcherService Service is disabled.");
                });
            }
        }
    }
}
