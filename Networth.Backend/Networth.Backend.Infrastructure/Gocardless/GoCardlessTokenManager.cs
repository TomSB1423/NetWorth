using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Networth.Backend.Infrastructure.Gocardless;

internal class GoCardlessTokenManager(IOptions<GocardlessOptions> options) : IDisposable
{
    private readonly HttpClient _httpClient = new();
    // Ensure that the base URL ends with a slash due to GoCardless redirection behavior
    private readonly string _baseUrl = options.Value.BankAccountDataBaseUrl + "/token/new/";
    private readonly SemaphoreSlim _tokenSemaphore = new(1, 1);
    private readonly GocardlessOptions _options = options.Value;
    private bool _disposed;

    private string? _accessToken;
    private DateTime _tokenExpiry;

    public async Task<string> GetValidTokenAsync(CancellationToken cancellationToken = default)
    {
        if (IsTokenValid())
        {
            return _accessToken!;
        }

        await _tokenSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (IsTokenValid())
            {
                return _accessToken!;
            }

            await RefreshTokenAsync(cancellationToken);

            if (string.IsNullOrEmpty(_accessToken))
            {
                throw new InvalidOperationException("Failed to obtain a valid access token from GoCardless API.");
            }

            return _accessToken;
        }
        finally
        {
            _tokenSemaphore.Release();
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _tokenSemaphore.Dispose();
            _httpClient.Dispose();
            _disposed = true;
        }
    }

    private async Task RefreshTokenAsync(CancellationToken cancellationToken)
    {
        var payload = new { secret_id = _options.SecretId, secret_key = _options.SecretKey };
        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl)
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

        // Handle potential null response from deserialization
        if (tokenResponse?.Access == null)
        {
            throw new InvalidOperationException("Invalid token response received from GoCardless API.");
        }

        _accessToken = tokenResponse.Access;
        var expiresInSeconds = tokenResponse.AccessExpires ?? 3600;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresInSeconds - 60);
    }

    private bool IsTokenValid()
    {
        return !string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry;
    }
}
