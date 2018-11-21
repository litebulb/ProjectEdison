using Edison.ChatService.Config;
using Edison.ChatService.Helpers;
using Edison.Common;
using Edison.Common.Config;
using Edison.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using AutoMapper;
using Edison.ChatService.Helpers.Interfaces;
using Edison.Core.Config;
using Edison.Core.Interfaces;
using Edison.Core;
using Edison.Common.DAO;
using Microsoft.Extensions.Options;

namespace Edison.ChatService
{
    public partial class Startup
    {
        ILogger<Startup> _logger = null;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Authentication
            services.AddAuthentication().AddAzureAdBearer(Configuration);
            services.AddMvc().AddControllersAsServices();

            services.AddOptions();
            services.AddApplicationInsightsKubernetesEnricher();
            services.AddApplicationInsightsTelemetry(Configuration);
            services.Configure<ServiceBusRabbitMQOptions>(Configuration.GetSection("ServiceBusRabbitMQ"));
            services.Configure<RestServiceOptions>(Configuration.GetSection("RestService"));
            services.Configure<BotOptions>(Configuration.GetSection("Bot"));
            services.Configure<AzureAdB2CWebOptions>(Configuration.GetSection("AzureAdB2CWeb"));
            services.Configure<CosmosDBOptions>(Configuration.GetSection("CosmosDb"));
            services.AddSingletonCosmosDBRepository<ChatUserSessionDAO>(Configuration["CosmosDb:Collections:Bot"]);
            services.AddSingletonCosmosDBRepository<ChatReportDAO>(Configuration["CosmosDb:Collections:ChatReports"]);
            services.AddSingleton<IDirectLineRestService>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotOptions>>().Value;
                var logger = sp.GetRequiredService<ILogger<RestServiceBase>>();
                return new DirectLineRestService(options.RestService.RestServiceUrl, options.RestService.SecretToken, logger);
            });
            services.AddSingleton<IDeviceRestService, DeviceRestService>();
            services.AddSingleton<IMassTransitServiceBus, ServiceBusRabbitMQ>();
            services.AddSingleton<IRoutingDataStore, BotRoutingDataStore>();
            services.AddSingleton<BotRoutingDataManager>();
            services.AddSingleton<ChatReportDataManager>();
            AddBot(services);

            //Automapper Can't use DI because all services are singletons
            Mapper.Initialize(cfg => { cfg.AddProfile<MappingProfile>(); });
            Mapper.AssertConfigurationIsValid();

            //Cors
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    );
            });

            //MVC
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // mirror logger messages to AppInsights
            _logger = loggerFactory.CreateLogger<Startup>();
            app.ApplicationServices.GetService<IMassTransitServiceBus>().Start(null);

            //Configure Bot
            ConfigureBot(app);

            //Enable Cors
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
