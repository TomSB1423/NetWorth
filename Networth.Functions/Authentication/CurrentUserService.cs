using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;

namespace Networth.Functions.Authentication;

/// <summary>
///     Implementation of <see cref="ICurrentUserService"/> for Azure Functions.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private ClaimsPrincipal? _user;

    /// <inheritdoc />
    public ClaimsPrincipal? User => _user;

    /// <inheritdoc />
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc />
    public string UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new InvalidOperationException("User is not authenticated");

    /// <inheritdoc />
    public string? Name => User?.FindFirst(ClaimTypes.Name)?.Value;

    /// <summary>
    ///     Sets the function context to extract user information from.
    /// </summary>
    /// <param name="context">The function context.</param>
    public void SetContext(FunctionContext context)
    {
        if (context.Items.TryGetValue("User", out object? user) && user is ClaimsPrincipal principal)
        {
            _user = principal;
        }
    }
}

