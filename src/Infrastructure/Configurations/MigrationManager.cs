namespace Lingtren.Infrastructure.Configurations
{
    using Lingtren.Infrastructure.Persistence;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class MigrationManager
    {
        public static WebApplication MigrateDatabase(this WebApplication webApp)
        {
            using (var scope = webApp.Services.CreateScope())
            using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                try
                {
                    appContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return webApp;
        }
    }
}