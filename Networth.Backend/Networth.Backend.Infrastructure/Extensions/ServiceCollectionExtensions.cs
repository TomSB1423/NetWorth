using System.Globalization;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Networth.Backend.Application.Commands;
using Networth.Backend.Application.Handlers;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Application.Validators;
using Networth.Backend.Infrastructure.Gocardless;
using Networth.Backend.Infrastructure.Gocardless.Auth;
using Networth.Backend.Infrastructure.Gocardless.Options;
using Newtonsoft.Json;
using Refit;

namespace Networth.Backend.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure GoCardless options from settings
        services.AddOptionsWithValidateOnStart<GocardlessOptions>()
            .Bind(configuration.GetSection(Constants.OptionsNames.GocardlessSection));

        // Configure HTTP client for GoCardless
        services.AddTransient<GoCardlessAuthHandler>();
        services.AddSingleton<GoCardlessTokenManager>(serviceProvider =>
        {
            IOptions<GocardlessOptions> options = serviceProvider.GetRequiredService<IOptions<GocardlessOptions>>();
            return new GoCardlessTokenManager(options);
        });

        services.AddRefitClient<IGocardlessClient>(_ =>
                new RefitSettings(
                    new NewtonsoftJsonContentSerializer(
                        new JsonSerializerSettings { Culture = CultureInfo.InvariantCulture, MissingMemberHandling = MissingMemberHandling.Ignore })))
            .ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                GocardlessOptions gocardlessOptions = serviceProvider.GetRequiredService<IOptions<GocardlessOptions>>().Value;
                httpClient.BaseAddress = new Uri(gocardlessOptions.BankAccountDataBaseUrl);
            })
            .AddHttpMessageHandler<GoCardlessAuthHandler>();

        services.AddTransient<IFinancialProvider, GocardlessService>();

        // Add application services
        services.AddTransient<ILinkAccountCommandHandler, LinkAccountCommandHandler>();

        // Add FluentValidation validators from Application layer
        services.AddTransient<IValidator<LinkAccountCommand>, LinkAccountCommandValidator>();

        return services;
    }
}
