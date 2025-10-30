namespace Destination.Infrastructure.Database.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        var pendingMigrations = await dbContext.Database
            .GetPendingMigrationsAsync(cancellationToken);

        var migrations = pendingMigrations.ToList();
        if (migrations.Count > 0)
        {
            logger.LogInformation("Applying {Count} pending migrations", migrations.Count);

            await dbContext.Database.MigrateAsync(cancellationToken);

            logger.LogInformation("Migrations applied successfully.");
        }
        else
        {
            logger.LogInformation("No pending migrations found.");
        }
    }
}
