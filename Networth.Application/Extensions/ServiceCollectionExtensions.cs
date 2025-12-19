using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Networth.Application.Commands;
using Networth.Application.Handlers;
using Networth.Application.Interfaces;
using Networth.Application.Options;
using Networth.Application.Queries;
using Networth.Application.Services;
using Networth.Application.Validators;

namespace Networth.Application.Extensions;

/// <summary>
///     Extension methods for registering application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds application services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register all validators from the Application assembly
        services.AddValidatorsFromAssemblyContaining<LinkInstitutionCommandValidator>();

        // Configure Frontend options
        services.AddOptions<FrontendOptions>()
            .Bind(configuration.GetSection(FrontendOptions.SectionName))
            .ValidateFluently()
            .ValidateOnStart();

        // Register simple mediator
        services.AddScoped<IMediator, Mediator>();

        // Register command handlers
        services.AddScoped<IRequestHandler<CreateUserCommand, CreateUserCommandResult>, CreateUserCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateUserCommand, UpdateUserCommandResult>, UpdateUserCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateAccountCommand, UpdateAccountCommandResult>, UpdateAccountCommandHandler>();
        services.AddScoped<IRequestHandler<LinkInstitutionCommand, LinkInstitutionCommandResult>, LinkInstitutionCommandHandler>();
        services.AddScoped<IRequestHandler<SyncAccountCommand, SyncAccountCommandResult>, SyncAccountCommandHandler>();
        services.AddScoped<IRequestHandler<SyncInstitutionCommand, SyncInstitutionCommandResult>, SyncInstitutionCommandHandler>();
        services.AddScoped<IRequestHandler<CalculateRunningBalanceCommand, CalculateRunningBalanceCommandResult>, CalculateRunningBalanceCommandHandler>();

        // Register query handlers
        services.AddScoped<IRequestHandler<GetAccountsQuery, GetAccountsQueryResult>, GetAccountsQueryHandler>();
        services.AddScoped<IRequestHandler<GetAccountQuery, GetAccountQueryResult>, GetAccountQueryHandler>();
        services.AddScoped<IRequestHandler<GetAccountBalancesQuery, GetAccountBalancesQueryResult>, GetAccountBalancesQueryHandler>();
        services.AddScoped<IRequestHandler<GetAccountDetailsQuery, GetAccountDetailsQueryResult>, GetAccountDetailsQueryHandler>();
        services.AddScoped<IRequestHandler<GetTransactionsQuery, GetTransactionsQueryResult>, GetTransactionsQueryHandler>();
        services.AddScoped<IRequestHandler<GetNetWorthHistoryQuery, GetNetWorthHistoryQueryResult>, GetNetWorthHistoryQueryHandler>();
        services.AddScoped<IRequestHandler<GetInstitutionsQuery, GetInstitutionsQueryResult>, GetInstitutionsQueryHandler>();
        services.AddScoped<IRequestHandler<GetRequisitionQuery, GetRequisitionQueryResult>, GetRequisitionQueryHandler>();

        return services;
    }
}
