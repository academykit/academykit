﻿namespace AcademyKit.Infrastructure.Configurations
{
    using AcademyKit.Infrastructure.Persistence;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class MigrationManager
    {
        public static WebApplication MigrateDatabase(this WebApplication webApp)
        {
            using (var scope = webApp.Services.CreateScope())
            using (
                var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>()
            )
            {
                try
                {
                    appContext.Database.Migrate();
                }
                catch
                {
                    throw;
                }
            }

            return webApp;
        }
    }
}
