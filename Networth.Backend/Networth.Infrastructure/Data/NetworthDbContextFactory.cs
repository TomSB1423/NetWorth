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

        string? connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__networth-db");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'networth-db' not found.");
        }

        optionsBuilder.UseNpgsql(connectionString);

        return new NetworthDbContext(optionsBuilder.Options);
    }
}
