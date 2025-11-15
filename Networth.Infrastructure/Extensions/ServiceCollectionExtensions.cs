using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Storage.Queues;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Networth.Application.Commands;
using Networth.Application.Handlers;
using Networth.Application.Interfaces;
using Networth.Application.Validators;
using Networth.Domain.Repositories;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Data.Options;
using Networth.Infrastructure.Data.Repositories;
using Networth.Infrastructure.Gocardless;
using Networth.Infrastructure.Gocardless.Auth;
using Networth.Infrastructure.Gocardless.Options;
using Networth.Infrastructure.Services;
using Npgsql;
using Refit;

namespace Networth.Infrastructure.Extensions;

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
