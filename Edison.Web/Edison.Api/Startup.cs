using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;
using Edison.Api.Config;
using Edison.Api.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Edison.Common.Interfaces;
using Edison.Common.Config;
using Edison.Common.DAO;
using Edison.Common;
using Microsoft.Bot.Connector;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Edison.Common.Chat.Config;
using Edison.Common.Chat.Models.Interface;
using Edison.Common.Chat.Repositories;

namespace Edison.Api
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
            //Authentication
            services.AddSingleton(_ => Configuration);
            var credentialProvider = new StaticCredentialProvider(
                Configuration.GetSection("BotConfigOptions:MicrosoftAppId")?.Value,
                Configuration.GetSection("BotConfigOptions:MicrosoftAppPassword")?.Value);
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddAzureAdAndB2CBearer(Configuration)
                .AddBotAuthentication(credentialProvider);

            //Options
            services.AddOptions();
            services.Configure<WebApiConfiguration>(Configuration.GetSection("WebApiConfiguration"));
            services.Configure<ServiceBusOptions>(Configuration.GetSection("ServiceBus"));
            services.Configure<BotOptions>(Configuration.GetSection("BotConfigOptions"));
            services.Configure<CosmosDBOptions>(typeof(EventClusterDAO).FullName, Configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(EventClusterDAO).FullName, opt => opt.Collection = opt.Collections.EventClusters);
            services.Configure<CosmosDBOptions>(typeof(DeviceDAO).FullName, Configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(DeviceDAO).FullName, opt => opt.Collection = opt.Collections.Devices);
            services.Configure<CosmosDBOptions>(typeof(ActionPlanDAO).FullName, Configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(ActionPlanDAO).FullName, opt => opt.Collection = opt.Collections.ActionPlans);
            services.Configure<CosmosDBOptions>(typeof(ResponseDAO).FullName, Configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(ResponseDAO).FullName, opt => opt.Collection = opt.Collections.Responses);
            services.Configure<CosmosDBOptions>(typeof(NotificationDAO).FullName, Configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(NotificationDAO).FullName, opt => opt.Collection = opt.Collections.Responses);

            //DI
            services.AddScoped(typeof(ICosmosDBRepository<>), typeof(CosmosDBRepository<>));
            services.AddSingleton<IServiceBusClient, RabbitMQServiceBus>();
            services.AddScoped<DevicesDataManager>();
            services.AddScoped<EventClustersDataManager>();
            services.AddScoped<ResponseDataManager>();
            services.AddScoped<ActionPlanDataManager>();
            services.AddScoped<IoTHubControllerDataManager>();
            services.AddScoped<NotificationHubDataManager>();
            services.AddSignalR().AddRedis(Configuration.GetValue<string>("SignalR:ConnectionString"));
            
            //Bot
            services.AddSingleton(typeof(ICredentialProvider), credentialProvider);
            services.AddSingleton<IConversationChatBot>(new ConversationChatBot());
            services.AddScoped<IChatRoutingDataManagerRepository, AzureTableStorageRoutingDataManager>(a =>
                 new AzureTableStorageRoutingDataManager(Configuration["BotConfigOptions:AzureStorageConnectionString"]));

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
            services
                .AddAutoMapper()
                .AddMvc(options =>
                {
                    options.Filters.Add(typeof(TrustServiceUrlAttribute));
                })
                .AddFluentValidation();

            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Edison.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<BotOptions> botOptions, IChatRoutingDataManagerRepository routingData,
            IConversationChatBot conversationChatBot)
        {

            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //Enable service bus as publisher only
            app.ApplicationServices.GetService<IServiceBusClient>().Start(null);

            //Enable Cors
            app.UseCors("CorsPolicy");

            //Enable SignalR
            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalRHub>("/signalr");
            });

            //Swagger
            //if (env.IsDevelopment())
            //{
            app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Edison.Api");
                });
            //}

            app.UseAuthentication();

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMvc();

            StartupMessageRouting.InitializeMessageRouting(botOptions, routingData,
                conversationChatBot);
        }
    }
}
