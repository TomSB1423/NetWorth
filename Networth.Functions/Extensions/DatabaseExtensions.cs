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
    /// Applies pending EF Core migrations to the database.
    /// </summary>
    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        await using NetworthDbContext dbContext = scope.ServiceProvider.GetRequiredService<NetworthDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    /// <summary>
    /// Ensures the mock user exists (development only).
    /// </summary>
    public static async Task EnsureMockUserAsync(this IServiceProvider serviceProvider)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        await using NetworthDbContext dbContext = scope.ServiceProvider.GetRequiredService<NetworthDbContext>();

        await EnsureMockUserExistsAsync(dbContext);
    }

    /// <summary>
    /// Ensures the mock development user exists in the database.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
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

    /// <summary>
    /// Ensures a user exists in the database, creating them if they don't exist.
    /// This is called on first authenticated request to auto-provision the user record.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="userId">The user ID from the authentication token.</param>
    /// <param name="userName">The user name from the authentication token (optional).</param>
    public static async Task EnsureUserExistsAsync(this NetworthDbContext dbContext, string userId, string? userName)
    {
        if (!await dbContext.Users.AnyAsync(u => u.Id == userId))
        {
            dbContext.Users.Add(new User
            {
                Id = userId,
                Name = userName ?? "Unknown User",
            });
            await dbContext.SaveChangesAsync();
        }
    }
}
