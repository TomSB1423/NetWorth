using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Networth.Functions.Authentication;

/// <summary>
///     Implementation of <see cref="ITokenValidationService"/> using Microsoft.Identity.Web.
/// </summary>
public class TokenValidationService : ITokenValidationService
{
    private readonly MicrosoftIdentityOptions _identityOptions;
    private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TokenValidationService"/> class.
    /// </summary>
    /// <param name="identityOptions">Microsoft Identity configuration options.</param>
    public TokenValidationService(IOptions<MicrosoftIdentityOptions> identityOptions)
    {
        _identityOptions = identityOptions.Value;
        _tokenHandler = new JwtSecurityTokenHandler();

        // Create the OpenID Connect configuration manager
        string metadataAddress = $"{_identityOptions.Instance}{_identityOptions.TenantId}/v2.0/.well-known/openid-configuration";
        _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            metadataAddress,
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever());
    }

    /// <inheritdoc />
    public async Task<ClaimsPrincipal> ValidateTokenAsync(string token)
    {
        // Get the OpenID Connect configuration
        OpenIdConnectConfiguration config = await _configurationManager.GetConfigurationAsync(CancellationToken.None);

        // Configure token validation parameters
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{_identityOptions.Instance}{_identityOptions.TenantId}/v2.0",
            ValidateAudience = true,
            ValidAudience = _identityOptions.ClientId,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = config.SigningKeys,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
        };

        // Validate the token
        ClaimsPrincipal principal = _tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

        return principal;
    }
}
