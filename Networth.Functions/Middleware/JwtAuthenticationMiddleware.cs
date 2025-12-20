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
    private readonly ILogger<JwtAuthenticationMiddleware> _logger;
    private readonly ConfigurationManager<OpenIdConnectConfiguration>? _configManager;
    private readonly TokenValidationParameters? _tokenValidationParameters;

    /// <summary>
    ///     Initializes a new instance of the <see cref="JwtAuthenticationMiddleware"/> class.
    /// </summary>
    public JwtAuthenticationMiddleware(IConfiguration configuration, ILogger<JwtAuthenticationMiddleware> logger)
    {
        _logger = logger;

        var azureAdSection = configuration.GetSection("AzureAd");
        if (!azureAdSection.Exists())
        {
            _logger.LogWarning("AzureAd configuration section is missing. JWT authentication will be skipped");
            return;
        }

        var tenantId = azureAdSection.GetValue<string>("TenantId");
        var clientId = azureAdSection.GetValue<string>("ClientId");

        if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId))
        {
            _logger.LogWarning("AzureAd TenantId or ClientId is missing. JWT authentication will be skipped");
            return;
        }

        var audience = azureAdSection.GetValue<string>("Audience") ?? clientId;
        var instance = azureAdSection.GetValue<string>("Instance") ?? "https://login.microsoftonline.com/";

        var metadataAddress = $"{instance}{tenantId}/v2.0/.well-known/openid-configuration";
        _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            metadataAddress,
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever());

        _tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudiences = [audience, $"api://{clientId}"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        _logger.LogInformation("JWT Authentication middleware initialized");
    }

    /// <inheritdoc />
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        if (_configManager is null || _tokenValidationParameters is null)
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

        if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authHeaderValues)
            || authHeaderValues.FirstOrDefault() is not { } authHeader
            || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogDebug("Missing or invalid Authorization header");
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var token = authHeader["Bearer ".Length..].Trim();

        try
        {
            var openIdConfig = await _configManager.GetConfigurationAsync(CancellationToken.None);
            var validationParams = _tokenValidationParameters.Clone();
            validationParams.IssuerSigningKeys = openIdConfig.SigningKeys;
            validationParams.ValidIssuer = openIdConfig.Issuer;

            var handler = new JwtSecurityTokenHandler();
            var result = await handler.ValidateTokenAsync(token, validationParams);

            if (result.IsValid && result.ClaimsIdentity is not null)
            {
                var principal = new ClaimsPrincipal(result.ClaimsIdentity);
                httpContext.User = principal;
                context.Items["User"] = principal;

                _logger.LogDebug("User authenticated successfully");
                await next(context);
            }
            else
            {
                _logger.LogDebug("Token validation failed");
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }
        catch (SecurityTokenValidationException)
        {
            _logger.LogDebug("Token validation failed");
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during authentication");
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}
