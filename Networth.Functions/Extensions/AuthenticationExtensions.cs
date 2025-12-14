using System.Security.Claims;
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
    ///     Uses Easy Auth in production and registers a mock user in development.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="environment">The host environment.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IHostEnvironment environment)
    {
        // Add HttpContextAccessor for accessing the current request context
        services.AddHttpContextAccessor();

        // Configure Easy Auth (App Service Authentication)
        services.AddAuthentication()
            .AddAppServicesAuthentication();

        // Register mock user only in development when Easy Auth is not enabled
        if (environment.IsDevelopment() &&
            !AppServicesAuthenticationInformation.IsAppServicesAadAuthenticationEnabled)
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
