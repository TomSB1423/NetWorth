using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Networth.Functions.Extensions;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Entities;

namespace Networth.Functions.Functions.Http.Development;

/// <summary>
/// Development-only endpoint to reset the database to its initial state.
/// </summary>
public class ResetDatabase
{
    private readonly ILogger<ResetDatabase> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetDatabase"/> class.
    /// </summary>
    public ResetDatabase(
        ILogger<ResetDatabase> logger,
        IServiceProvider serviceProvider,
        IHostEnvironment environment)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _environment = environment;
    }

    /// <summary>
    /// Resets the database by deleting all data and recreating the schema.
    /// Only available in Development environment.
    /// </summary>
    [Function("ResetDatabase")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "dev/reset-database")] HttpRequestData req)
    {
        // Only allow in Development environment
        if (!_environment.IsDevelopment())
        {
            _logger.LogWarning("Attempt to reset database in non-development environment");
            HttpResponseData forbiddenResponse = req.CreateResponse(HttpStatusCode.Forbidden);
            await forbiddenResponse.WriteStringAsync("This endpoint is only available in Development environment");
            return forbiddenResponse;
        }

        try
        {
            _logger.LogInformation("Resetting database...");

            // Use a scoped DbContext for the reset operation
            using IServiceScope scope = _serviceProvider.CreateScope();
            NetworthDbContext dbContext = scope.ServiceProvider.GetRequiredService<NetworthDbContext>();

            // Delete the database
            await dbContext.Database.EnsureDeletedAsync();
            _logger.LogInformation("Database deleted");

            // Recreate the database
            await dbContext.Database.EnsureCreatedAsync();
            _logger.LogInformation("Database recreated");

            // Create a new scope with a fresh DbContext after database recreation
            using IServiceScope newScope = _serviceProvider.CreateScope();
            NetworthDbContext freshDbContext = newScope.ServiceProvider.GetRequiredService<NetworthDbContext>();

            // Ensure mock user exists using the shared extension method
            await freshDbContext.EnsureMockUserExistsAsync();
            _logger.LogInformation("Mock user created");

            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                message = "Database reset successfully",
                timestamp = DateTime.UtcNow,
            });

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reset database");
            HttpResponseData errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Failed to reset database: {ex.Message}");
            return errorResponse;
        }
    }
}
