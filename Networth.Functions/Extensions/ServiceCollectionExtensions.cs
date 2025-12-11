using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Networth.Application.Interfaces;
using Networth.Functions.Authentication;
using Serilog;

namespace Networth.Functions.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFunctionsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog(config => { config.ReadFrom.Configuration(configuration); });

        services.AddApplicationInsightsTelemetryWorkerService();

        services.Configure<MicrosoftIdentityOptions>(configuration.GetSection("AzureAd"));

        services.AddOptions<AuthenticationOptions>()
            .BindConfiguration(AuthenticationOptions.SectionName)
            .ValidateDataAnnotations();

        services.AddSingleton<ITokenValidationService, TokenValidationService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
