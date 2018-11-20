using Edison.Common.Interfaces;
using Edison.Common.Config;
using Edison.Common;
using Edison.Core.Interfaces;
using Edison.Core.Config;
using Edison.Core;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Edison.DeviceSynchronizationService.Consumers;

namespace Edison.DeviceSynchronizationService
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
            services.AddApplicationInsightsKubernetesEnricher();
            services.AddApplicationInsightsTelemetry(Configuration);
            services.Configure<RestServiceOptions>(Configuration.GetSection("RestService"));
            services.Configure<ServiceBusRabbitMQOptions>(Configuration.GetSection("ServiceBusRabbitMQ"));

            services.AddMassTransit(c =>
            {
                c.AddConsumer<DeviceCreateOrUpdateRequestedConsumer>();
                c.AddConsumer<DeviceDeleteRequestedConsumer>();
            });
            services.AddScoped<DeviceCreateOrUpdateRequestedConsumer>();
            services.AddScoped<DeviceDeleteRequestedConsumer>();
            services.AddSingleton<IDeviceRestService, DeviceRestService>();
            services.AddSingleton<IMassTransitServiceBus, ServiceBusRabbitMQ>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // mirror logger messages to AppInsights
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Debug);
            app.ApplicationServices.GetService<IMassTransitServiceBus>().Start(ep =>
            {
                ep.LoadFrom(app.ApplicationServices);
            });

            app.Run(context =>
            {
                return context.Response.WriteAsync("DeviceManagement Service is running...");
            });
        }
    }
}
