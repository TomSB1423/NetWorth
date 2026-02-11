using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Networth.Functions.Authentication;
using Networth.Functions.Options;

namespace Networth.Functions.Extensions;

/// <summary>
///     Extension methods for configuring authentication.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    ///     Configures authentication for the application.
    ///     Uses Firebase token validation in production and mock user header injection in development.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<NetworthOptions>()
            .Bind(configuration.GetSection(NetworthOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<FirebaseOptions>()
            .Bind(configuration.GetSection(FirebaseOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Add HttpContextAccessor for accessing the current request context
        services.AddHttpContextAccessor();

        // Add FunctionContextAccessor for accessing the FunctionContext from scoped services
        services.AddSingleton<IFunctionContextAccessor, FunctionContextAccessor>();

        // Register the current user service
        // Mock users are handled by MockUserMiddleware via X-Mock-User header in development
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
