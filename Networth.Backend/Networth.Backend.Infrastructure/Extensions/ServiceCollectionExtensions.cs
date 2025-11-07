using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Networth.Backend.Application.Commands;
using Networth.Backend.Application.Handlers;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Application.Validators;
using Networth.Backend.Infrastructure.Data.Context;
using Networth.Backend.Infrastructure.Data.Options;
using Networth.Backend.Infrastructure.Gocardless;
using Networth.Backend.Infrastructure.Gocardless.Auth;
using Networth.Backend.Infrastructure.Gocardless.Options;
using Npgsql;
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

        services.AddSingleton<RefitRetryHandler>();

        JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
            },
        };

        services.AddRefitClient<IGocardlessClient>(_ =>
                new RefitSettings { ContentSerializer = new SystemTextJsonContentSerializer(options) })
            .ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                GocardlessOptions gocardlessOptions = serviceProvider
                    .GetRequiredService<IOptions<GocardlessOptions>>().Value;
                httpClient.BaseAddress = new Uri(gocardlessOptions.BankAccountDataBaseUrl);
            })
            .AddHttpMessageHandler<GoCardlessAuthHandler>()
            .AddHttpMessageHandler(serviceProvider => new RefitRetryHandler(serviceProvider.GetRequiredService<ILogger<RefitRetryHandler>>()));

        // Register Infrastructure
        services.AddTransient<IFinancialProvider, GocardlessService>();

        // Register Application services
        services.AddTransient<LinkAccountCommandHandler>();
        services.AddTransient<IValidator<LinkAccountCommand>, LinkAccountCommandValidator>();

        // Use DB - with Aspire NpgsqlDataSource
        services.AddDbContext<NetworthDbContext>((serviceProvider, dbContextOptionsBuilder) =>
        {
            NpgsqlDataSource dataSource = serviceProvider.GetRequiredService<NpgsqlDataSource>();
            dbContextOptionsBuilder.UseNpgsql(dataSource, npgsqlOptions =>
            {
                npgsqlOptions.CommandTimeout(30);
            });
        });

        services.AddScoped<IDbConnection>(sp =>
        {
            NpgsqlDataSource dataSource = sp.GetRequiredService<NpgsqlDataSource>();
            return dataSource.OpenConnection();
        });
        return services;
    }
}
