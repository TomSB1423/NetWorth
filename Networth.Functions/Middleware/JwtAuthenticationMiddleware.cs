using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Networth.Functions.Middleware;

/// <summary>
///     Middleware that validates JWT Bearer tokens from Entra ID.
///     This middleware runs before Azure Functions and populates HttpContext.User
///     with the claims from the validated token.
/// </summary>
public class JwtAuthenticationMiddleware : IFunctionsWorkerMiddleware
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtAuthenticationMiddleware> _logger;
    private readonly ConfigurationManager<OpenIdConnectConfiguration> _configManager;
    private readonly TokenValidationParameters _tokenValidationParameters;

    /// <summary>
    ///     Initializes a new instance of the <see cref="JwtAuthenticationMiddleware"/> class.
    /// </summary>
    public JwtAuthenticationMiddleware(IConfiguration configuration, ILogger<JwtAuthenticationMiddleware> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var tenantId = _configuration["AzureAd:TenantId"];
        var clientId = _configuration["AzureAd:ClientId"];
        var audience = _configuration["AzureAd:Audience"] ?? clientId;
        var instance = _configuration["AzureAd:Instance"] ?? "https://login.microsoftonline.com/";

        if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId))
        {
            _logger.LogWarning("AzureAd configuration is missing. JWT authentication will be skipped.");
            _configManager = null!;
            _tokenValidationParameters = null!;
            return;
        }

        var metadataAddress = $"{instance}{tenantId}/v2.0/.well-known/openid-configuration";
        _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            metadataAddress,
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever());

        _tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{instance}{tenantId}/v2.0",
            ValidateAudience = true,
            ValidAudiences = [audience, $"api://{clientId}"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        _logger.LogInformation(
            "JWT Authentication middleware initialized for tenant {TenantId}, audience {Audience}",
            tenantId,
            audience);
    }

    /// <inheritdoc />
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        // Skip authentication if not configured
        if (_configManager == null)
        {
            await next(context);
            return;
        }

        var httpContext = context.GetHttpContext();
        if (httpContext == null)
        {
            // Not an HTTP trigger, skip authentication
            await next(context);
            return;
        }

        var authHeader = httpContext.Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("No Bearer token found in request");
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var token = authHeader["Bearer ".Length..].Trim();

        try
        {
            var openIdConfig = await _configManager.GetConfigurationAsync(CancellationToken.None);
            var validationParams = _tokenValidationParameters.Clone();
            validationParams.IssuerSigningKeys = openIdConfig.SigningKeys;

            var handler = new JwtSecurityTokenHandler();
            var result = await handler.ValidateTokenAsync(token, validationParams);

            if (result.IsValid && result.ClaimsIdentity != null)
            {
                var principal = new ClaimsPrincipal(result.ClaimsIdentity);

                // Set the authenticated user on HttpContext
                httpContext.User = principal;

                // Also store in FunctionContext.Items for access by scoped services
                // This is necessary because IHttpContextAccessor may not reflect middleware changes
                context.Items["User"] = principal;

                var userId = result.ClaimsIdentity.FindFirst("oid")?.Value
                             ?? result.ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogDebug("Successfully authenticated user {UserId}", userId);

                await next(context);
            }
            else
            {
                _logger.LogWarning("Token validation failed: {Error}", result.Exception?.Message);
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }
        catch (SecurityTokenValidationException ex)
        {
            _logger.LogWarning(ex, "Token validation failed: {Message}", ex.Message);
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during token validation");
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}
