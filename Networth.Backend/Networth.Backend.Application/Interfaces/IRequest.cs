namespace Networth.Backend.Application.Interfaces;

/// <summary>
///     Marker interface for requests.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IRequest<out TResponse>
{
}
