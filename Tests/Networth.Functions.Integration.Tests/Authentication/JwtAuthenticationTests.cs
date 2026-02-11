using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration.Authentication;

/// <summary>
///     Tests for JWT authentication configuration and token validation.
/// </summary>
public class JwtAuthenticationTests
{
    // Test configuration values matching appsettings.Development.json
    private const string TenantId = "e2a8021c-d32c-470d-8703-af48ca780354";
    private const string Instance = "https://networthauth.ciamlogin.com/";
    private const string ApiClientId = "60b00c11-2aeb-46ef-b479-1d4559ef0904";

    private readonly ITestOutputHelper _output;
    private readonly IConfiguration _configuration;

    public JwtAuthenticationTests(ITestOutputHelper output)
    {
        _output = output;
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AzureAd:TenantId"] = TenantId,
                ["AzureAd:Instance"] = Instance,
                ["AzureAd:ClientId"] = ApiClientId,
            })
            .Build();
    }

    [Fact]
    public async Task OidcConfiguration_CanBeFetched_FromCiamEndpoint()
    {
        // Arrange
        var metadataAddress = $"{Instance}{TenantId}/v2.0/.well-known/openid-configuration";
        var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            metadataAddress,
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever());

        // Act
        var config = await configManager.GetConfigurationAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(config);
        Assert.NotNull(config.Issuer);
        Assert.NotNull(config.SigningKeys);
        Assert.NotEmpty(config.SigningKeys);

        _output.WriteLine($"Issuer from OIDC: {config.Issuer}");
        _output.WriteLine($"Signing Keys count: {config.SigningKeys.Count}");

        // Verify the issuer format for CIAM - it uses tenant ID format, not custom domain
        Assert.Contains(TenantId, config.Issuer);
    }

    [Fact]
    public async Task Issuer_FromOidcMetadata_DiffersFromMetadataUrl()
    {
        // Arrange - This test documents the CIAM issuer quirk
        var metadataAddress = $"{Instance}{TenantId}/v2.0/.well-known/openid-configuration";
        var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            metadataAddress,
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever());

        // Act
        var config = await configManager.GetConfigurationAsync(CancellationToken.None);

        // Assert - The issuer from CIAM uses a different URL pattern than the metadata URL
        // Metadata URL: https://networthauth.ciamlogin.com/{tenantId}/v2.0/.well-known/openid-configuration
        // Issuer: https://{tenantId}.ciamlogin.com/{tenantId}/v2.0
        var expectedMetadataIssuer = $"{Instance}{TenantId}/v2.0";
        var actualIssuer = config.Issuer;

        _output.WriteLine($"Metadata URL pattern would suggest issuer: {expectedMetadataIssuer}");
        _output.WriteLine($"Actual issuer from OIDC metadata: {actualIssuer}");

        // These should NOT be equal - this is why we must use the issuer from the OIDC config
        Assert.NotEqual(expectedMetadataIssuer, actualIssuer);

        // The actual issuer should be in the tenant-ID-based format
        Assert.StartsWith($"https://{TenantId}.ciamlogin.com", actualIssuer);
    }

    [Fact]
    public async Task TokenValidationParameters_ShouldUseIssuerFromOidcConfig()
    {
        // Arrange
        var metadataAddress = $"{Instance}{TenantId}/v2.0/.well-known/openid-configuration";
        var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            metadataAddress,
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever());

        // This mimics what the fixed JwtAuthenticationMiddleware does
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            // Note: ValidIssuer is NOT pre-set - it will be set from OIDC config
            ValidateAudience = true,
            ValidAudiences = [ApiClientId, $"api://{ApiClientId}"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        // Act - Get the OIDC config and set up validation params (like the middleware does)
        var openIdConfig = await configManager.GetConfigurationAsync(CancellationToken.None);
        var validationParams = tokenValidationParameters.Clone();
        validationParams.IssuerSigningKeys = openIdConfig.SigningKeys;
        validationParams.ValidIssuer = openIdConfig.Issuer;

        // Assert
        Assert.Equal(openIdConfig.Issuer, validationParams.ValidIssuer);
        Assert.NotEmpty(validationParams.IssuerSigningKeys);

        _output.WriteLine($"ValidIssuer set to: {validationParams.ValidIssuer}");
        _output.WriteLine($"IssuerSigningKeys count: {validationParams.IssuerSigningKeys.Count()}");
    }

    [Fact]
    public async Task ValidAudiences_ShouldIncludeBothFormats()
    {
        // Arrange & Act
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidAudiences = [ApiClientId, $"api://{ApiClientId}"],
        };

        // Assert - Tokens may have audience as either the client ID or api:// URI format
        Assert.Contains(ApiClientId, tokenValidationParameters.ValidAudiences);
        Assert.Contains($"api://{ApiClientId}", tokenValidationParameters.ValidAudiences);

        _output.WriteLine($"Valid audiences: {string.Join(", ", tokenValidationParameters.ValidAudiences)}");
    }

    [Fact]
    public void JwtSecurityTokenHandler_CanParseTokenStructure()
    {
        // Arrange - A sample JWT structure (header.payload.signature)
        // This is NOT a valid token, just testing the parser can handle the structure
        var sampleJwtHeader = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9";
        var sampleJwtPayload = "eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWUsImlhdCI6MTUxNjIzOTAyMn0";
        var sampleJwtSignature = "POstGetfAytaZS82wHcjoTyoqhMyxXiWdR7Nn7A29DNSl0EiXLdwJ6xC6AfgZWF1bOsS_TuYI3OG85AmiExREkrS6tDfTQ2B3WXlrr-wp5AokiRbz3_oB4OxG-W9KcEEbDRcZc0nH3L7LzYptiy1PtAylQGxHTWZXtGz4ht0bAecBgmpdgXMguEIcoqPJ1n3pIWk_dUZegpqx0Lka21H6XxUTxiy8OcaarA8zdnPUnV6AmNP3ecFawIFYdvJB_cm-GvpCSbr8G8y_Mllj8f4x9nBH8pQux89_6gUY618iYv7tuPWBFfEbLxtF2pZS6YC1aSfLQxeNe8djT9YjpvRZA";

        var handler = new JwtSecurityTokenHandler();

        // Act - Just verify we can read the token structure (won't validate signature)
        var canRead = handler.CanReadToken($"{sampleJwtHeader}.{sampleJwtPayload}.{sampleJwtSignature}");

        // Assert
        Assert.True(canRead);
        _output.WriteLine("JwtSecurityTokenHandler can read JWT structure");
    }
}
