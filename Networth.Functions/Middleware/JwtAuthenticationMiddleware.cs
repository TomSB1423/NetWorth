using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Networth.Functions.Options;

namespace Networth.Functions.Middleware;

/// <summary>
/// Validates Firebase ID tokens using standard JWT validation.
/// No service account required - uses Google's public signing keys.
/// </summary>
public class JwtAuthenticationMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<JwtAuthenticationMiddleware> _logger;
    private readonly TokenValidationParameters? _tokenValidationParameters;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtAuthenticationMiddleware"/> class.
    /// </summary>
    /// <param name="firebaseOptions">The firebase options.</param>
    /// <param name="logger">The logger.</param>
    public JwtAuthenticationMiddleware(
        IOptions<FirebaseOptions> firebaseOptions,
        ILogger<JwtAuthenticationMiddleware> logger)
    {
        _logger = logger;

        try
        {
#pragma warning disable VSTHRD002 // Avoid synchronous waits - Required for constructor initialization
            var signingKeys = FetchGoogleSigningKeysAsync().GetAwaiter().GetResult();
#pragma warning restore VSTHRD002

            var projectId = firebaseOptions.Value.ProjectId;
            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = $"https://securetoken.google.com/{projectId}",
                ValidateAudience = true,
                ValidAudience = projectId,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = signingKeys
            };

            _logger.LogInformation("Firebase JWT authentication initialized for project: {ProjectId}", projectId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Firebase JWT authentication");
        }
    }

    /// <inheritdoc />
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        if (_tokenValidationParameters is null)
        {
            await next(context);
            return;
        }

        var httpContext = context.GetHttpContext();
        if (httpContext is null)
        {
            await next(context);
            return;
        }

        // Skip authentication for OPTIONS preflight requests (CORS)
        if (httpContext.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authHeaderValues)
            || authHeaderValues.FirstOrDefault() is not { } authHeader
            || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Unauthenticated request to {Function}", context.FunctionDefinition.Name);
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var token = authHeader["Bearer ".Length..].Trim();

        try
        {
            var result = await _tokenHandler.ValidateTokenAsync(token, _tokenValidationParameters);

            if (!result.IsValid || result.SecurityToken is not JwtSecurityToken jwt)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, jwt.Subject),
                new("sub", jwt.Subject)
            };

            AddClaimIfPresent(jwt, claims, "email", ClaimTypes.Email);
            AddClaimIfPresent(jwt, claims, "name", ClaimTypes.Name);
            AddClaimIfPresent(jwt, claims, "picture");
            AddClaimIfPresent(jwt, claims, "email_verified");
            AddClaimIfPresent(jwt, claims, "user_id");

            var identity = new ClaimsIdentity(claims, "Firebase");
            httpContext.User = new ClaimsPrincipal(identity);
            context.Items["User"] = httpContext.User;

            _logger.LogDebug("Authenticated user: {UserId}", jwt.Subject);
            await next(context);
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogDebug("Token validation failed: {Message}", ex.Message);
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during authentication");
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
    }

#pragma warning disable VSTHRD002 // Avoid synchronous waits - Required for constructor initialization
    private static async Task<IEnumerable<SecurityKey>> FetchGoogleSigningKeysAsync()
    {
        using var client = new HttpClient();

        var jwkJson = await client
            .GetStringAsync("https://www.googleapis.com/service_accounts/v1/jwk/securetoken@system.gserviceaccount.com")
            .ConfigureAwait(false);

        return new JsonWebKeySet(jwkJson).GetSigningKeys();
    }
#pragma warning restore VSTHRD002

    private static void AddClaimIfPresent(JwtSecurityToken jwt, List<Claim> claims, string claimType, string? mappedType = null)
    {
        var claim = jwt.Claims.FirstOrDefault(c => c.Type == claimType);
        if (claim is not null)
        {
            claims.Add(new Claim(claimType, claim.Value));
            if (mappedType is not null)
            {
                claims.Add(new Claim(mappedType, claim.Value));
            }
        }
    }
}
