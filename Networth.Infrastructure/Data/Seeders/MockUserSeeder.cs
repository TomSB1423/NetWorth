using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Networth.Domain.Entities;
using Networth.Infrastructure.Data.Context;

namespace Networth.Infrastructure.Data.Seeders;

/// <summary>
///     Seeds the database with mock user data for development.
/// </summary>
public static class MockUserSeeder
{
    /// <summary>
    ///     Seeds the mock user into the database if it doesn't exist.
    /// </summary>
    /// <param name="host">The application host.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SeedMockUserAsync(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        ILogger<NetworthDbContext> logger = services.GetRequiredService<ILogger<NetworthDbContext>>();
        NetworthDbContext context = services.GetRequiredService<NetworthDbContext>();

        try
        {
            logger.LogInformation("Starting database migration and seeding...");

            // Apply pending migrations with timeout
            using CancellationTokenSource migrationCts = new(TimeSpan.FromMinutes(2));
            await context.Database.MigrateAsync(migrationCts.Token);

            logger.LogInformation("Database migration completed successfully");

            // Check if mock user already exists
            const string mockUserId = "mock-user-123";
            bool userExists = await context.Users.AnyAsync(u => u.Id == mockUserId, migrationCts.Token);

            if (!userExists)
            {
                User mockUser = new()
                {
                    Id = mockUserId,
                    Name = "Mock Development User",
                };

                await context.Users.AddAsync(mockUser, migrationCts.Token);
                await context.SaveChangesAsync(migrationCts.Token);

                logger.LogInformation("Mock user seeded successfully");
            }
            else
            {
                logger.LogInformation("Mock user already exists");
            }
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Database migration or seeding timed out");
            throw; // Re-throw to prevent app from starting with incomplete database
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating database or seeding the mock user");
            throw; // Re-throw to prevent app from starting with incomplete database
        }
    }
}

