namespace Lingtren.Infrastructure.Configurations
{
    using Lingtren.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    public static class ServiceConfigurationExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql
            (configuration.GetConnectionString("DefaultConnection"), MySqlServerVersion.LatestSupportedServerVersion),
            ServiceLifetime.Scoped);

            return services;
        }
    }
}

