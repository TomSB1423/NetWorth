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
        services.AddOptionsWithValidateOnStart<GocardlessOptions>(Constants.OptionsNames.GocardlessSection);

        // Configure HTTP client for GoCardless
        services.AddScoped<GoCardlessTokenManager>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<GocardlessOptions>>();
            return new GoCardlessTokenManager(options, null!);
        });

        services.AddRefitClient<GocardlessService>(_ => new RefitSettings(new NewtonsoftJsonContentSerializer(
                new JsonSerializerSettings()
                {
                    Culture = CultureInfo.InvariantCulture,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                })))
            .ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                var gocardlessOptions = serviceProvider.GetRequiredService<IOptions<GocardlessOptions>>().Value;
                httpClient.BaseAddress = new Uri(gocardlessOptions.BankAccountDataBaseUrl);
                httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                httpClient.DefaultRequestHeaders.Add("GoCardless-Version", Gocardless.Constants.GocardlessBankAccountDataApiVersion);
            })
            .AddHttpMessageHandler<GoCardlessAuthHandler>();

        services.AddTransient<IFinancialProvider, GocardlessService>();
        return services;
    }
}
