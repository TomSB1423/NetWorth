using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Networth.Application.Extensions;
using Networth.Application.Interfaces;
using Networth.Domain.Repositories;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Repositories;
using Networth.Infrastructure.Gocardless;
using Networth.Infrastructure.Gocardless.Auth;
using Networth.Infrastructure.Gocardless.Handlers;
using Networth.Infrastructure.Gocardless.Options;
using Networth.Infrastructure.Services;
using Npgsql;
using Polly;
using Refit;

namespace Networth.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure GoCardless options from settings with FluentValidation
        services.AddSingleton<IValidator<GocardlessOptions>, GocardlessOptionsValidator>();

        services.AddOptions<GocardlessOptions>()
            .Bind(configuration.GetSection(Constants.OptionsNames.GocardlessSection))
            .ValidateFluently();

        // Configure HTTP client for GoCardless
        services.AddTransient<GoCardlessAuthHandler>();
        services.AddTransient<RateLimitLoggingHandler>();
        services.AddSingleton<GoCardlessTokenManager>(serviceProvider =>
        {
            IOptions<GocardlessOptions> options = serviceProvider.GetRequiredService<IOptions<GocardlessOptions>>();
            return new GoCardlessTokenManager(options);
        });

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

        var refitClientBuilder = services.AddRefitClient<IGocardlessClient>(_ =>
                new RefitSettings { ContentSerializer = new SystemTextJsonContentSerializer(options) })
            .ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                GocardlessOptions gocardlessOptions = serviceProvider
                    .GetRequiredService<IOptions<GocardlessOptions>>().Value;
                httpClient.BaseAddress = new Uri(gocardlessOptions.BankAccountDataBaseUrl);
            })
            .AddHttpMessageHandler<GoCardlessAuthHandler>();

        refitClientBuilder.AddStandardResilienceHandler(options =>
            {
                options.Retry.BackoffType = DelayBackoffType.Exponential;
                options.Retry.UseJitter = true;
                options.Retry.MaxRetryAttempts = 5;
                options.Retry.ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(response =>
                        response.StatusCode == System.Net.HttpStatusCode.Conflict ||
                        response.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                        (int)response.StatusCode >= 500);
            });

        // Add rate limit logging handler after resilience handler so it logs every attempt (inner handler)
        refitClientBuilder.AddHttpMessageHandler<RateLimitLoggingHandler>();

        // Register Infrastructure services
        services.AddTransient<IFinancialProvider, GocardlessService>();
        services.AddScoped<IQueueService, QueueService>();

        // Register repositories
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAgreementRepository, AgreementRepository>();
        services.AddScoped<IRequisitionRepository, RequisitionRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ICacheMetadataRepository, CacheMetadataRepository>();
        services.AddScoped<IInstitutionMetadataRepository, InstitutionMetadataRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

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
