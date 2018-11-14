using Edison.Common;
using Edison.Common.Config;
using Edison.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddSingletonCosmosDBRepository<T>(this IServiceCollection services, string name) where T : class, IEntityDAO
        {
            services.AddSingleton<ICosmosDBRepository<T>>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<CosmosDBOptions>>().Value;
                var logger = sp.GetRequiredService<ILogger<CosmosDBRepository<T>>>();
                return new CosmosDBRepository<T>(options.Endpoint, options.AuthKey, options.Database, name, logger);
            });
            return services;
        }
    }
}
