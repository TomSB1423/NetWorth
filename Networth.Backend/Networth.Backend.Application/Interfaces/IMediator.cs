namespace Networth.Backend.Application.Interfaces;

/// <summary>
///     Simple mediator interface.
/// </summary>
public interface IMediator
{
    /// <summary>
    ///     Sends a request, validates it, and executes the handler.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default);
}
