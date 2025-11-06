using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;

namespace Networth.Backend.Functions.Authentication;

/// <summary>
///     Implementation of <see cref="ICurrentUserService"/> for Azure Functions.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly FunctionContext _context;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CurrentUserService"/> class.
    /// </summary>
    /// <param name="context">The function context.</param>
    public CurrentUserService(FunctionContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public ClaimsPrincipal? User
    {
        get
        {
            if (_context.Items.TryGetValue("User", out object? user) && user is ClaimsPrincipal principal)
            {
                return principal;
            }

            return null;
        }
    }

    /// <inheritdoc />
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc />
    public string UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new InvalidOperationException("User is not authenticated");

    /// <inheritdoc />
    public string Email => User?.FindFirst(ClaimTypes.Email)?.Value
        ?? throw new InvalidOperationException("User email not found");

    /// <inheritdoc />
    public string? Name => User?.FindFirst(ClaimTypes.Name)?.Value;
}

