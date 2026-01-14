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
    /// Ensures a user exists in the database, creating them if they don't exist.
    /// This is called on first authenticated request to auto-provision the user record.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="firebaseUid">The Firebase UID from the authentication token.</param>
    /// <param name="userName">The user name from the authentication token (optional).</param>
    public static async Task EnsureUserExistsAsync(this NetworthDbContext dbContext, string firebaseUid, string? userName)
    {
        if (!await dbContext.Users.AnyAsync(u => u.FirebaseUid == firebaseUid))
        {
            dbContext.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                FirebaseUid = firebaseUid,
                Name = userName ?? "Unknown User",
                CreatedAt = DateTime.UtcNow,
            });
            await dbContext.SaveChangesAsync();
        }
    }
}
