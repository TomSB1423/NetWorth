using System.Security.Claims;

namespace Networth.Functions.Authentication;

/// <summary>
///     Service interface for validating JWT Bearer tokens.
/// </summary>
public interface ITokenValidationService
{
    /// <summary>
    ///     Validates a JWT Bearer token and returns the ClaimsPrincipal.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>The ClaimsPrincipal containing the validated claims.</returns>
    Task<ClaimsPrincipal> ValidateTokenAsync(string token);
}
