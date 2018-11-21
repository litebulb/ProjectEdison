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
using System.Security.Claims;

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
            services
                .AddAuthentication()
                .AddAzureAdAndB2CBearer(Configuration);
            //Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireAssertion(context => context.User.HasClaim(c =>
                    (c.Type == ClaimTypes.Role && c.Value == "Admin") ||
                    (c.Type == "appidacr" && c.Value == "1") ||
                    (c.Type == "azpacr" && c.Value == "1"))));

                options.AddPolicy("SuperAdmin", policy => policy.RequireAssertion(context => context.User.HasClaim(c => 
                    (c.Type == "appidacr" && c.Value == "1") || 
                    (c.Type == "azpacr" && c.Value == "1"))));

                options.AddPolicy("Consumer", policy => policy.RequireAuthenticatedUser());
            });

            //Options
            services.AddOptions();
            services.AddApplicationInsightsKubernetesEnricher();
            services.AddApplicationInsightsTelemetry(Configuration);
            services.Configure<WebApiOptions>(Configuration.GetSection("WebApiConfiguration"));
            services.Configure<NotificationsOptions>(Configuration.GetSection("NotificationHub"));
            services.Configure<CosmosDBOptions>(typeof(EventClusterDAO).FullName, Configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(EventClusterDAO).FullName, opt => opt.Collection = opt.Collections.EventClusters);
            services.Configure<CosmosDBOptions>(typeof(DeviceDAO).FullName, Configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(DeviceDAO).FullName, opt => opt.Collection = opt.Collections.Devices);
            services.Configure<CosmosDBOptions>(typeof(ActionPlanDAO).FullName, Configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(ActionPlanDAO).FullName, opt => opt.Collection = opt.Collections.ActionPlans);
            services.Configure<CosmosDBOptions>(typeof(ResponseDAO).FullName, Configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(ResponseDAO).FullName, opt => opt.Collection = opt.Collections.Responses);
            services.Configure<CosmosDBOptions>(typeof(NotificationDAO).FullName, Configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(NotificationDAO).FullName, opt => opt.Collection = opt.Collections.Notifications);
            services.Configure<CosmosDBOptions>(typeof(ChatReportDAO).FullName, Configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(ChatReportDAO).FullName, opt => opt.Collection = opt.Collections.ChatReports);

            //Service Bus
            services.Configure<ServiceBusRabbitMQOptions>(Configuration.GetSection("ServiceBusRabbitMQ"));
            services.AddSingleton<IMassTransitServiceBus, ServiceBusRabbitMQ>();
            //services.Configure<ServiceBusAzureOptions>(Configuration.GetSection("ServiceBusAzure"));
            //services.AddSingleton<IMassTransitServiceBus, ServiceBusAzure>();

            //DI
            services.AddScoped(typeof(ICosmosDBRepository<>), typeof(CosmosDBRepository<>));
            services.AddScoped<DevicesDataManager>();
            services.AddScoped<EventClustersDataManager>();
            services.AddScoped<ResponseDataManager>();
            services.AddScoped<ActionPlanDataManager>();
            services.AddScoped<IoTHubControllerDataManager>();
            services.AddScoped<NotificationHubDataManager>();
            services.AddScoped<ReportDataManager>();
            services.AddSignalR().AddRedis(Configuration.GetValue<string>("SignalR:ConnectionString"));

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
                .AddMvc()
                .AddFluentValidation();

            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Edison.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.ApplicationServices.GetService<IMassTransitServiceBus>().Start(null);

            //Enable Cors
            app.UseCors("CorsPolicy");

            //Swagger
            //if (env.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Edison.Api");
            });
            //}

            //Middleware for supporting SignalR authentication
            app.UseMiddleware(typeof(SignalRMiddleware));

            app.UseAuthentication();

            //Enable SignalR
            app.UseSignalR(routes => 
            {
                routes.MapHub<SignalRHub>("/signalr");
            });

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMvc();
        }
    }
}
