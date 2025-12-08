using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Entities;

namespace Networth.Functions.Extensions;

/// <summary>
/// Extension methods for database setup and initialization.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Ensures the database exists and sets up initial data (mock user, queues).
    /// </summary>
    public static async Task EnsureDatabaseSetupAsync(this IServiceProvider serviceProvider)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        await using NetworthDbContext dbContext = scope.ServiceProvider.GetRequiredService<NetworthDbContext>();

        // Ensure database exists
        await dbContext.Database.EnsureCreatedAsync();

        // Ensure mock user exists
        await EnsureMockUserExistsAsync(dbContext);
    }

    /// <summary>
    /// Ensures the mock development user exists in the database.
    /// </summary>
    public static async Task EnsureMockUserExistsAsync(this NetworthDbContext dbContext)
    {
        const string mockUserId = "mock-user-123";
        if (!await dbContext.Users.AnyAsync(u => u.Id == mockUserId))
        {
            dbContext.Users.Add(new User
            {
                Id = mockUserId,
                Name = "Mock Development User",
            });
            await dbContext.SaveChangesAsync();
        }
    }
}
