using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace Networth.Backend.Functions.Configuration;

/// <summary>
///     Configuration options for OpenAPI documentation.
/// </summary>
public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
{
    public override OpenApiInfo Info { get; set; } = new()
    {
        Version = "1.0.0",
        Title = "Networth Backend API",
        Description = "API for managing financial data and networth calculations",
        Contact = new OpenApiContact { Name = "Networth Team", Email = "support@networth.com" }
    };

    public override OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;
}
