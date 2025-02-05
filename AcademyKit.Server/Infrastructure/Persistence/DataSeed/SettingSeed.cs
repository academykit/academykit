using AcademyKit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AcademyKit.Infrastructure.Persistence.DataSeed;

/// <summary>
/// SettingSeed class is used to seed the settings data into the database.
/// </summary>
public static class SettingSeed
{
    /// <summary>
    /// SeedAsync method is used to seed the settings data into the database.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task SeedAsync(
        ApplicationDbContext context,
        ILogger logger,
        CancellationToken cancellationToken
    )
    {
        if (!await context.Settings.AnyAsync(cancellationToken))
        {
            var settingsData = new List<Setting>
            {
                new Setting { Key = "Storage", Value = "Server" },
                new Setting { Key = "AWS_AccessKey", Value = null },
                new Setting { Key = "AWS_SecretKey", Value = null },
                new Setting { Key = "AWS_FileBucket", Value = null },
                new Setting { Key = "AWS_VideoBucket", Value = null },
                new Setting { Key = "AWS_CloudFront", Value = null },
                new Setting { Key = "AWS_RegionEndpoint", Value = null },
                new Setting { Key = "Server_Url", Value = null },
                new Setting { Key = "Server_Bucket", Value = null },
                new Setting { Key = "Server_AccessKey", Value = null },
                new Setting { Key = "Server_SecretKey", Value = null },
                new Setting { Key = "Server_PresignedExpiryTime", Value = null },
                new Setting { Key = "Server_EndPoint", Value = null },
                new Setting { Key = "Server_PresignedUrl", Value = null }
            };
            await context.Settings.AddRangeAsync(settingsData, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Settings data seeded successfully");
        }
    }
}
