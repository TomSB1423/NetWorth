using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Networth.Backend.Domain.Entities;
using Networth.Backend.Infrastructure.Data.Context;

namespace Networth.Backend.Infrastructure.Data.Seeders;

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
            // Apply pending migrations
            await context.Database.MigrateAsync();

            // Check if mock user already exists
            const string mockUserId = "mock-user-123";
            bool userExists = await context.Users.AnyAsync(u => u.Id == mockUserId);

            if (!userExists)
            {
                User mockUser = new()
                {
                    Id = mockUserId,
                    Name = "Mock Development User",
                };

                await context.Users.AddAsync(mockUser);
                await context.SaveChangesAsync();

                logger.LogInformation("Mock user seeded successfully");
            }
            else
            {
                logger.LogInformation("Mock user already exists");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the mock user");
        }
    }
}

