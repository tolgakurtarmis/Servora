using Microsoft.EntityFrameworkCore;
using Servora.Infrastructure.Persistence;

namespace Servora.Api.Startup;

public static class DatabaseStartup
{
    public static async Task InitializeAsync(WebApplication app, CancellationToken cancellationToken = default)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ServoraDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("Servora.Api.DatabaseStartup");

        await WaitForDatabaseAsync(db, logger, cancellationToken);
        await db.Database.MigrateAsync(cancellationToken);
        logger.LogInformation("Database migrations applied.");
    }

    private static async Task WaitForDatabaseAsync(
        ServoraDbContext db,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        const int maxAttempts = 30;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                if (await db.Database.CanConnectAsync(cancellationToken))
                {
                    return;
                }
            }
            catch (Exception exception) when (attempt < maxAttempts)
            {
                logger.LogWarning(
                    exception,
                    "Waiting for database connection (attempt {Attempt}/{MaxAttempts}).",
                    attempt,
                    maxAttempts);
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }

        throw new InvalidOperationException("Could not connect to the database.");
    }
}
