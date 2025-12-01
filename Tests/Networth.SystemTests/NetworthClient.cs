using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Networth.Functions.Models.Responses;

namespace Networth.SystemTests;

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
    public async Task<SyncInstitutionResponse> SyncInstitutionAsync(
        string institutionId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync(
            $"/api/institutions/{institutionId}/sync",
            null,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<SyncInstitutionResponse>(
            _jsonOptions,
            cancellationToken);

        if (result == null)
        {
            throw new InvalidOperationException("Failed to deserialize sync institution response");
        }

        return result;
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

        response.EnsureSuccessStatusCode();

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
}
