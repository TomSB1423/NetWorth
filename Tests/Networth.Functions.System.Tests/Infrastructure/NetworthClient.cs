using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Networth.Functions.Models.Responses;

namespace Networth.SystemTests.Infrastructure;

/// <summary>
///     Strongly-typed client for the Networth API that handles serialization and status code assertions.
/// </summary>
public class NetworthClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NetworthClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for API calls.</param>
    public NetworthClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }

    /// <summary>
    ///     Syncs all accounts for a specific institution.
    /// </summary>
    /// <param name="institutionId">The institution ID to sync.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sync result.</returns>
    public async Task SyncInstitutionAsync(
        string institutionId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync(
            $"/api/institutions/{institutionId}/sync",
            null,
            cancellationToken);

        await EnsureSuccessStatusCodeWithBodyAsync(response, cancellationToken);
    }

    /// <summary>
    ///     Gets all available institutions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of institutions.</returns>
    public async Task<List<InstitutionResponse>> GetInstitutionsAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/api/institutions", cancellationToken);

        await EnsureSuccessStatusCodeWithBodyAsync(response, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        using var jsonDoc = JsonDocument.Parse(content);

        // Handle both array and object with "value" property
        var institutionsElement = jsonDoc.RootElement.ValueKind == JsonValueKind.Array
            ? jsonDoc.RootElement
            : jsonDoc.RootElement.GetProperty("value");

        var institutions = JsonSerializer.Deserialize<List<InstitutionResponse>>(
            institutionsElement.GetRawText(),
            _jsonOptions);

        return institutions ?? new List<InstitutionResponse>();
    }

    /// <summary>
    ///     Links a bank account by creating an agreement and requisition.
    /// </summary>
    /// <param name="institutionId">The institution ID to link.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The link account response with authorization link.</returns>
    public async Task<LinkAccountResponse> LinkAccountAsync(
        string institutionId,
        CancellationToken cancellationToken = default)
    {
        var requestBody = new { institutionId };

        var response = await _httpClient.PostAsJsonAsync(
            "/api/account/link",
            requestBody,
            _jsonOptions,
            cancellationToken);

        await EnsureSuccessStatusCodeWithBodyAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<LinkAccountResponse>(
            _jsonOptions,
            cancellationToken);

        if (result == null)
        {
            throw new InvalidOperationException("Failed to deserialize link account response");
        }

        return result;
    }

    /// <summary>
    ///     Ensures the response has a success status code, including the response body in the exception if not.
    /// </summary>
    private static async Task EnsureSuccessStatusCodeWithBodyAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}). Response body: {body}");
        }
    }
}
