using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Edison.Core;
using Edison.Core.Interfaces;
using Edison.Core.Config;
using Edison.Common;
using Edison.Common.Interfaces;
using Edison.Common.Config;
using Edison.ResponseService.Consumers;

namespace Edison.ResponseService
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
                c.AddConsumer<ResponseTagExistingEventClustersRequestedConsumer>();
                c.AddConsumer<ResponseTagNewEventClusterRequestedConsumer>();
                c.AddConsumer<ResponseActionLightSensorEventConsumer>();
                c.AddConsumer<ResponseActionNotificationEventConsumer>();
                c.AddConsumer<ResponseActionEventConsumer>();
            });
            services.AddScoped<ResponseTagExistingEventClustersRequestedConsumer>();
            services.AddScoped<ResponseTagNewEventClusterRequestedConsumer>();
            services.AddScoped<ResponseActionLightSensorEventConsumer>();
            services.AddScoped<ResponseActionNotificationEventConsumer>();
            services.AddScoped<ResponseActionEventConsumer>();
            services.AddSingleton<IEventClusterRestService, EventClusterRestService>();
            services.AddSingleton<INotificationRestService, NotificationRestService>();
            services.AddSingleton<IDeviceRestService, DeviceRestService>();
            services.AddSingleton<IIoTHubControllerRestService, IoTHubControllerRestService>();
            services.AddSingleton<IResponseRestService, ResponseRestService>();
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
                return context.Response.WriteAsync("Response Service is running...");
            });
        }
    }
}
