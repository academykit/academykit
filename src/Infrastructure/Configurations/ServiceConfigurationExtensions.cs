namespace Lingtren.Infrastructure.Configurations
{
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Persistence;
    using Lingtren.Infrastructure.Services;
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

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<IEmailService, EmailService>();

            return services;
        }
    }
}

