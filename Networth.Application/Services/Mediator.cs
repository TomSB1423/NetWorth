using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;

namespace Networth.Application.Services;

/// <summary>
///     Simple mediator implementation that validates and executes handlers.
/// </summary>
public class Mediator(IServiceProvider serviceProvider, ILogger<Mediator> logger) : IMediator
{
    public async Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Processing request {RequestType}", typeof(TRequest).Name);

        // Get and run validator if exists
        var validator = serviceProvider.GetService<IValidator<TRequest>>();
        if (validator != null)
        {
            logger.LogDebug("Validating request {RequestType}", typeof(TRequest).Name);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                logger.LogWarning(
                    "Validation failed for {RequestType}: {Errors}",
                    typeof(TRequest).Name,
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new ValidationException(validationResult.Errors);
            }
        }

        // Get and execute handler
        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        logger.LogDebug("Executing handler for {RequestType}", typeof(TRequest).Name);

        return await handler.HandleAsync(request, cancellationToken);
    }
}
