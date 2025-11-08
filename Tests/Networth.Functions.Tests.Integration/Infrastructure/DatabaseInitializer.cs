using Microsoft.EntityFrameworkCore;
using Networth.Infrastructure.Data.Context;
using Npgsql;

namespace Networth.Functions.Tests.Integration.Infrastructure;

/// <summary>
///     Handles database initialization for integration tests.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    ///     Ensures the database is created and seeded for tests.
    ///     Uses EnsureCreatedAsync which automatically applies HasData seed configuration.
    /// </summary>
    /// <param name="app">The distributed application.</param>
    /// <param name="databaseResourceName">The name of the database resource.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task InitializeDatabaseForTestsAsync(DistributedApplication app, string databaseResourceName)
    {
        // Get the connection string for the database resource
        string? connectionString = await app.GetConnectionStringAsync(databaseResourceName);
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Could not get connection string for database resource '{databaseResourceName}'");
        }

        // First, create the database if it doesn't exist using raw SQL
        await CreateDatabaseIfNotExistsAsync(connectionString);

        // Now create the schema and seed data using EF Core
        DbContextOptions<NetworthDbContext> options = new DbContextOptionsBuilder<NetworthDbContext>()
            .UseNpgsql(connectionString)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        await using NetworthDbContext context = new(options);

        // EnsureCreatedAsync will create the schema and automatically seed
        // the mock user defined via HasData
        await context.Database.EnsureCreatedAsync();
    }

    private static async Task CreateDatabaseIfNotExistsAsync(string connectionString)
    {
        // Parse the connection string to get database name and create a connection to postgres database
        NpgsqlConnectionStringBuilder builder = new(connectionString);
        string? databaseName = builder.Database;

        if (string.IsNullOrEmpty(databaseName))
        {
            throw new InvalidOperationException("Database name not found in connection string");
        }

        builder.Database = "postgres"; // Connect to default postgres database

        await using NpgsqlConnection connection = new(builder.ConnectionString);
        await connection.OpenAsync();

        // Check if database exists
        await using NpgsqlCommand checkCmd = new($"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'", connection);
        object? exists = await checkCmd.ExecuteScalarAsync();

        if (exists == null)
        {
            // Create database
            await using NpgsqlCommand createCmd = new($"CREATE DATABASE \"{databaseName}\"", connection);
            await createCmd.ExecuteNonQueryAsync();
        }
    }
}
