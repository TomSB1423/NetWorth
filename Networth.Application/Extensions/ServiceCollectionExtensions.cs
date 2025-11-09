using System.Reflection;
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
        // Register simple mediator
        services.AddScoped<IMediator, Mediator>();

        // Register handlers
        services.AddScoped<IRequestHandler<GetTransactionsQuery, GetTransactionsQueryResult>, GetTransactionsQueryHandler>();
        services.AddScoped<IRequestHandler<LinkAccountCommand, LinkAccountCommandResult>, LinkAccountCommandHandler>();

        // Register validators
        services.AddScoped<IValidator<GetTransactionsQuery>, GetTransactionsQueryValidator>();
        services.AddScoped<IValidator<LinkAccountCommand>, LinkAccountCommandValidator>();

        return services;
    }
}
