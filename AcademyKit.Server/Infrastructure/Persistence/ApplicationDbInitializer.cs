using AcademyKit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AcademyKit.Infrastructure.Persistence;

public class ApplicationDbInitializer : IHostedService
{
    private readonly ILogger<ApplicationDbInitializer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ApplicationDbInitializer(
        ILogger<ApplicationDbInitializer> logger,
        IServiceProvider serviceProvider
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var strategy = context.Database.CreateExecutionStrategy();
        _logger.LogInformation("Running migrations for {Context}", nameof(ApplicationDbContext));
        await strategy.ExecuteAsync(async () =>
        {
            await context.Database.MigrateAsync(cancellationToken: cancellationToken);
            await SeedDataAsync(context, cancellationToken);
        });
        _logger.LogInformation("Migrations applied and data seeded successfully");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task SeedDataAsync(
        ApplicationDbContext context,
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
            _logger.LogInformation("Settings data seeded successfully");
        }
    }
}
