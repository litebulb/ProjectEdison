using Edison.Common.Interfaces;
using Edison.Common.Config;
using Edison.Common;
using GreenPipes;
using MassTransit;
using MassTransit.DocumentDbIntegration;
using MassTransit.DocumentDbIntegration.Saga;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Edison.Workflows.Config;

namespace Edison.Workflows
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
            services.Configure<WorkflowConfig>(Configuration.GetSection("WorkflowsConfig"));
            services.Configure<ServiceBusRabbitMQOptions>(Configuration.GetSection("ServiceBusRabbitMQ"));
            services.AddSingleton<IMassTransitServiceBus, ServiceBusRabbitMQ>();
            services.AddSingleton<EventProcessingStateMachine>();
            services.AddSingleton<DeviceSynchronizationStateMachine>();
            services.AddSingleton<ResponseStateMachine>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // mirror logger messages to AppInsights
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Debug);

            //Configure persistence for Sagas
            Uri endpointUri = new Uri(Configuration.GetValue<string>("CosmosDb:Endpoint"));
            var repositoryEventProcessing = new DocumentDbSagaRepository<EventProcessingState>(
                new DocumentClient(endpointUri, Configuration.GetValue<string>("CosmosDb:AuthKey")),
                Configuration.GetValue<string>("CosmosDb:Database"),
                Configuration.GetValue<string>("CosmosDb:Collection"));
            var repositoryDeviceSync = new DocumentDbSagaRepository<DeviceSynchronizationState>(
                new DocumentClient(endpointUri, Configuration.GetValue<string>("CosmosDb:AuthKey")),
                Configuration.GetValue<string>("CosmosDb:Database"),
                Configuration.GetValue<string>("CosmosDb:Collection"));
            var repositoryResponseSync = new DocumentDbSagaRepository<ResponseState>(
                new DocumentClient(endpointUri, Configuration.GetValue<string>("CosmosDb:AuthKey")),
                Configuration.GetValue<string>("CosmosDb:Database"),
                Configuration.GetValue<string>("CosmosDb:Collection"));

            app.ApplicationServices.GetService<IMassTransitServiceBus>().Start(ep =>
            {
                ep.UseRetry(p => {
                    p.Interval(10, TimeSpan.FromMilliseconds(200));
                    p.Handle<DocumentDbConcurrencyException>();
                    p.Handle<Automatonymous.UnhandledEventException>();
                });
                ep.UseInMemoryOutbox();
                ep.StateMachineSaga(app.ApplicationServices.GetService<EventProcessingStateMachine>(), repositoryEventProcessing);
                ep.StateMachineSaga(app.ApplicationServices.GetService<DeviceSynchronizationStateMachine>(), repositoryDeviceSync);
                ep.StateMachineSaga(app.ApplicationServices.GetService<ResponseStateMachine>(), repositoryResponseSync);
            }, true);

            app.Run(context =>
            {
                return context.Response.WriteAsync("EventProcessor Tracking Service is running...");
            });
        }
    }
}
