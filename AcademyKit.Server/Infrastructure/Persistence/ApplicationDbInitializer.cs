using AcademyKit.Infrastructure.Persistence.DataSeed;
using Microsoft.EntityFrameworkCore;

namespace AcademyKit.Infrastructure.Persistence;

/// <summary>
/// ApplicationDbInitializer class is used to initialize the database.
/// </summary>
public class ApplicationDbInitializer : IHostedService
{
    private readonly ILogger<ApplicationDbInitializer> _logger;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Constructor for ApplicationDbInitializer.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public ApplicationDbInitializer(
        ILogger<ApplicationDbInitializer> logger,
        IServiceProvider serviceProvider
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// StartAsync method is used to start the database initialization.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
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

    /// <summary>
    /// StopAsync method is used to stop the database initialization.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>
    /// SeedDataAsync method is used to seed the data into the database.
    /// </summary>
    /// <param name="context">The database context.</param>
    private async Task SeedDataAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        await using (
            var transaction = await context.Database.BeginTransactionAsync(cancellationToken)
        )
        {
            try
            {
                await SettingSeed.SeedAsync(context, _logger, cancellationToken);
                await MailTemplateSeed.SeedAsync(context, _logger, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred during data seeding. Rolling back transaction."
                );
                await transaction.RollbackAsync(cancellationToken);
            }
        }
    }
}
