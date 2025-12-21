using System.Text.Json;
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
    /// Ensures the sandbox institution exists (development only).
    /// </summary>
    public static async Task EnsureSandboxInstitutionAsync(this IServiceProvider serviceProvider)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        await using NetworthDbContext dbContext = scope.ServiceProvider.GetRequiredService<NetworthDbContext>();

        await EnsureSandboxInstitutionExistsAsync(dbContext);
    }

    /// <summary>
    /// Ensures the sandbox institution exists in the database.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public static async Task EnsureSandboxInstitutionExistsAsync(this NetworthDbContext dbContext)
    {
        const string sandboxId = "SANDBOXFINANCE_SFIN0000";
        if (!await dbContext.Institutions.AnyAsync(i => i.Id == sandboxId))
        {
            dbContext.Institutions.Add(new InstitutionMetadata
            {
                Id = sandboxId,
                Name = "Sandbox Finance",
                LogoUrl = "https://cdn.iconscout.com/icon/free/png-256/free-code-sandbox-logo-icon-svg-download-png-3031688.png",
                CountryCode = "GB",
                Countries = JsonSerializer.Serialize(new[] { "GB" }),
                LastUpdated = DateTime.UtcNow,
            });
            await dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Ensures the mock development user exists in the database.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public static async Task EnsureMockUserExistsAsync(this NetworthDbContext dbContext)
    {
        const string mockFirebaseUid = "mock-user-123";
        if (!await dbContext.Users.AnyAsync(u => u.FirebaseUid == mockFirebaseUid))
        {
            dbContext.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                FirebaseUid = mockFirebaseUid,
                Name = "Mock Development User",
                CreatedAt = DateTime.UtcNow,
            });
            await dbContext.SaveChangesAsync();
        }
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
