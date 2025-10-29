using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Networth.Backend.Infrastructure.Data.Context;

namespace Networth.Backend.Infrastructure.Data;

/// <summary>
///     Factory for creating NetworthDbContext instances at design time (for migrations).
/// </summary>
public class NetworthDbContextFactory : IDesignTimeDbContextFactory<NetworthDbContext>
{
    /// <inheritdoc />
    public NetworthDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<NetworthDbContext> optionsBuilder = new();

        // Default connection string for design-time migrations
        // Note: With Aspire's dynamic ports, run migrations while Aspire is running
        // and use the ConnectionStrings__networth-db environment variable, or use this fallback
        string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__networth-db")
                                  ?? "Host=localhost;Port=5432;Database=networth-db;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);

        return new NetworthDbContext(optionsBuilder.Options);
    }
}
