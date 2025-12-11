using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Networth.Functions.Authentication;
using Serilog;

namespace Networth.Functions.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFunctionsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog(config => { config.ReadFrom.Configuration(configuration); }, writeToProviders: true);

        services.AddApplicationInsightsTelemetryWorkerService();

        services.AddOptions<NetworthAuthenticationOptions>()
            .BindConfiguration(NetworthAuthenticationOptions.SectionName)
            .ValidateDataAnnotations();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
