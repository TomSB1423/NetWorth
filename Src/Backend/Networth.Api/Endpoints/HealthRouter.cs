using Networth.Interfaces;

namespace Networth.Endpoints;

public class HealthRouter : IFeatureRouter
{
    public void RegisterRoutes(IEndpointRouteBuilder app)
    {
        var healthGroup = app.MapGroup("/health")
            .WithTags("Health");

        healthGroup.MapGet("/", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
            .WithName("GetHealth")
            .WithSummary("Check the health status of the API")
            .WithDescription("Returns the current health status of the API service")
            .WithOpenApi();
    }
}