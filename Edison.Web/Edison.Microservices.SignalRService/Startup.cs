using Edison.Common.Interfaces;
using Edison.Common.Config;
using Edison.Common;
using Edison.Core.Interfaces;
using Edison.Core.Config;
using Edison.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Edison.SignalRService.Consumers;
using Microsoft.Extensions.Logging;

namespace Edison.SignalRService
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
            services.Configure<RestServiceOptions>(Configuration.GetSection("RestService"));
            services.Configure<ServiceBusOptions>(Configuration.GetSection("ServiceBus"));
            services.AddMassTransit(c =>
            {
                c.AddConsumer<EventUIUpdateConsumer>();
                c.AddConsumer<DeviceUIUpdateConsumer>();
                c.AddConsumer<ResponseUIUpdateConsumer>();
            });
            services.AddScoped<EventUIUpdateConsumer>();
            services.AddScoped<DeviceUIUpdateConsumer>();
            services.AddScoped<ResponseUIUpdateConsumer>();
            services.AddSingleton<ISignalRRestService, SignalRRestService>();
            services.AddSingleton<IServiceBusClient, RabbitMQServiceBus>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // mirror logger messages to AppInsights
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Debug);
            app.ApplicationServices.GetService<IServiceBusClient>().Start(ep =>
            {
                ep.LoadFrom(app.ApplicationServices);
            });

            app.Run(context =>
            {
                return context.Response.WriteAsync("SignalRService Service is running...");
            });
        }
    }
}
