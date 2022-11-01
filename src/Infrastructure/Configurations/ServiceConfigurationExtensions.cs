namespace Lingtren.Infrastructure.Configurations
{
    using Lingtren.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    internal static class ServiceConfigurationExtensions
    {
        internal static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnectionString = configuration.GetConnectionString("DefaultConnection");
            var serverVersion = MySqlServerVersion.LatestSupportedServerVersion;

            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(dbConnectionString, serverVersion),
            ServiceLifetime.Scoped);

            return services;
        }
    }
}

