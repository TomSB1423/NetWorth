using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Networth.Backend.Application.Commands;
using Networth.Backend.Application.Handlers;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Application.Queries;
using Networth.Backend.Application.Services;
using Networth.Backend.Application.Validators;

namespace Networth.Backend.Application.Extensions;

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
