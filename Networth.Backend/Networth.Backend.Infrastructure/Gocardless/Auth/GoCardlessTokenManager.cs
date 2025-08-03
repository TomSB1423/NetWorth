using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Networth.Backend.Infrastructure.Gocardless.DTOs;
using Networth.Backend.Infrastructure.Gocardless.Options;

namespace Networth.Backend.Infrastructure.Gocardless.Auth;

internal class GoCardlessTokenManager(IOptions<GocardlessOptions> options) : IDisposable
{
    // Ensure that the base URL ends with a slash due to GoCardless redirection behavior
    private readonly string _baseUrl = options.Value.BankAccountDataBaseUrl + "/token/new/";
    private readonly HttpClient _httpClient = new();
    private readonly GocardlessOptions _options = options.Value;
    private readonly SemaphoreSlim _tokenSemaphore = new(1, 1);

    private string? _accessToken;
    private bool _disposed;
    private DateTime _tokenExpiry;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _tokenSemaphore.Dispose();
        _httpClient.Dispose();
        _disposed = true;
    }

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

    private async Task RefreshTokenAsync(CancellationToken cancellationToken)
    {
        var payload = new { secret_id = _options.SecretId, secret_key = _options.SecretKey };
        string json = JsonSerializer.Serialize(payload);
        using StringContent content = new(json, Encoding.UTF8, "application/json");

        HttpRequestMessage request = new(HttpMethod.Post, _baseUrl) { Content = content };

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        string responseContent = await response.Content.ReadAsStringAsync();
        GetTokenResponseDto? tokenResponse = JsonSerializer.Deserialize<GetTokenResponseDto>(
            responseContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

        // Handle potential null response from deserialization

        _accessToken = tokenResponse?.Access ?? throw new InvalidOperationException("Invalid token response received from GoCardless API.");
        int expiresInSeconds = tokenResponse.AccessExpires ?? 3600;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresInSeconds - 60);
    }

    private bool IsTokenValid() => !string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry;
}
