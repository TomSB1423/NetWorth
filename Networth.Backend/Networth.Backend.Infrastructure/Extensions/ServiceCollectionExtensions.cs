using System.Globalization;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Infrastructure.Gocardless;
using Newtonsoft.Json;
using Refit;

namespace Networth.Backend.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure GoCardless options from settings
        services.AddOptionsWithValidateOnStart<GocardlessOptions>()
            .Bind(configuration.GetSection(Constants.OptionsNames.GocardlessSection))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.BankAccountDataBaseUrl),
                "BankAccountDataBaseUrl configuration is required")
            .Validate(
                options => Uri.TryCreate(options.BankAccountDataBaseUrl, UriKind.Absolute, out _),
                "BankAccountDataBaseUrl must be a valid absolute URI");

        // Configure HTTP client for GoCardless
        services.AddTransient<GoCardlessAuthHandler>();
        services.AddSingleton<GoCardlessTokenManager>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<GocardlessOptions>>();
            return new GoCardlessTokenManager(options);
        });

        services.AddRefitClient<IGocardlessClient>(_ =>
                new RefitSettings(
                    new NewtonsoftJsonContentSerializer(
                        new JsonSerializerSettings()
                        {
                            Culture = CultureInfo.InvariantCulture,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                        })))
            .ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                var gocardlessOptions = serviceProvider.GetRequiredService<IOptions<GocardlessOptions>>().Value;
                httpClient.BaseAddress = new Uri(gocardlessOptions.BankAccountDataBaseUrl);
            })
            .AddHttpMessageHandler<GoCardlessAuthHandler>();

        services.AddTransient<IFinancialProvider, GocardlessService>();
        return services;
    }
}
