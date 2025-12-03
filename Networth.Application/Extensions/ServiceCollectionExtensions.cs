using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Networth.Application.Commands;
using Networth.Application.Handlers;
using Networth.Application.Interfaces;
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
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register all validators from the Application assembly
        services.AddValidatorsFromAssemblyContaining<LinkAccountCommandValidator>();

        // Register simple mediator
        services.AddScoped<IMediator, Mediator>();

        // Register command handlers
        services.AddScoped<IRequestHandler<LinkAccountCommand, LinkAccountCommandResult>, LinkAccountCommandHandler>();
        services.AddScoped<IRequestHandler<SyncAccountCommand, SyncAccountCommandResult>, SyncAccountCommandHandler>();
        services.AddScoped<IRequestHandler<SyncInstitutionCommand, SyncInstitutionCommandResult>, SyncInstitutionCommandHandler>();

        // Register query handlers
        services.AddScoped<IRequestHandler<GetAccountsQuery, GetAccountsQueryResult>, GetAccountsQueryHandler>();
        services.AddScoped<IRequestHandler<GetAccountQuery, GetAccountQueryResult>, GetAccountQueryHandler>();
        services.AddScoped<IRequestHandler<GetAccountBalancesQuery, GetAccountBalancesQueryResult>, GetAccountBalancesQueryHandler>();
        services.AddScoped<IRequestHandler<GetAccountDetailsQuery, GetAccountDetailsQueryResult>, GetAccountDetailsQueryHandler>();
        services.AddScoped<IRequestHandler<GetTransactionsQuery, GetTransactionsQueryResult>, GetTransactionsQueryHandler>();
        services.AddScoped<IRequestHandler<GetInstitutionsQuery, GetInstitutionsQueryResult>, GetInstitutionsQueryHandler>();
        services.AddScoped<IRequestHandler<GetRequisitionQuery, GetRequisitionQueryResult>, GetRequisitionQueryHandler>();

        return services;
    }
}
