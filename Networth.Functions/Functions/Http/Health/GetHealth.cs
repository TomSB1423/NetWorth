using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Networth.Functions.Functions.Http.Health;

/// <summary>
///     Health check endpoint for the Azure Functions app.
///     This endpoint is used by Aspire to determine when the Functions app is ready to accept traffic.
/// </summary>
[AllowAnonymous]
public sealed class GetHealth
{
    /// <summary>
    ///     Returns a healthy status indicating the Functions app is ready.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <returns>An OK result with "Healthy" status.</returns>
    [Function("GetHealth")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest req)
    {
        return new OkObjectResult("Healthy");
    }
}
