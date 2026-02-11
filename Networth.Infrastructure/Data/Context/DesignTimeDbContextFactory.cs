using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Networth.Infrastructure.Data.Context;

/// <summary>
/// Design-time factory for creating DbContext instances for EF Core migrations.
/// This is used by dotnet ef commands when creating and applying migrations.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<NetworthDbContext>
{
    /// <summary>
    /// Creates a new instance of the DbContext for design-time operations.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>A configured DbContext instance.</returns>
    public NetworthDbContext CreateDbContext(string[] args)
    {
        // Build configuration from appsettings and environment variables
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        DbContextOptionsBuilder<NetworthDbContext> optionsBuilder = new();

        // Use connection string from configuration, or fallback to local development
        string connectionString = configuration.GetConnectionString("networth-db")
            ?? "Host=localhost;Database=networth-db;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(typeof(NetworthDbContext).Assembly.FullName);
        });

        return new NetworthDbContext(optionsBuilder.Options);
    }
}
