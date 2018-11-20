using Edison.DeviceProvisioning.Config;
using Edison.DeviceProvisioning.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Edison.DeviceProvisionning
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
            services.AddAuthentication().AddAzureAdBearer(Configuration);
            //Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireAssertion(context => context.User.HasClaim(c =>
                    (c.Type == ClaimTypes.Role && c.Value == "Admin"))));
            });


            services.AddOptions();
            services.AddApplicationInsightsKubernetesEnricher();
            services.Configure<DeviceProvisioningOptions>(Configuration.GetSection("DeviceProvisioning"));
            services.Configure<AzureAdOptions>(Configuration.GetSection("RestService:AzureAd"));
            //services.AddSingleton<CertificateGenerator>();
            services.AddSingleton<CertificateCsrSignator>();
            services.AddSingleton<KeyVaultManager>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
