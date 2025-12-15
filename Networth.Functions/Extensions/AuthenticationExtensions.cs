using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Networth.Functions.Authentication;

namespace Networth.Functions.Extensions;

/// <summary>
///     Extension methods for configuring authentication.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    ///     Configures authentication for the application.
    ///     Uses JWT Bearer token validation in production and registers a mock user in development.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="environment">The host environment.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Add HttpContextAccessor for accessing the current request context
        services.AddHttpContextAccessor();

        // Add FunctionContextAccessor for accessing the FunctionContext from scoped services
        services.AddSingleton<IFunctionContextAccessor, FunctionContextAccessor>();

        // Configure JWT Bearer authentication using Microsoft Identity Web
        // This validates tokens issued by Entra ID for our API app registration
        services.AddAuthentication()
            .AddMicrosoftIdentityWebApi(configuration, "AzureAd");

        // Register mock user only in development when AzureAd is not configured
        if (environment.IsDevelopment() && string.IsNullOrEmpty(configuration["AzureAd:ClientId"]))
        {
            services.AddScoped<ClaimsPrincipal>(_ =>
            {
                ClaimsIdentity identity = new(
                    [
                        new Claim(ClaimTypes.NameIdentifier, "mock-user-123"),
                        new Claim(ClaimTypes.Name, "Mock Development User"),
                        new Claim("IsActive", "true"),
                    ],
                    "MockAuthentication");
                return new ClaimsPrincipal(identity);
            });
        }

        // Register the current user service
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
