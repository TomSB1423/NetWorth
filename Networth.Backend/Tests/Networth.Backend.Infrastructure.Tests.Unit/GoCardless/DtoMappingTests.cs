using System.Text.Json;
using Networth.Backend.Infrastructure.Gocardless.DTOs;

namespace Networth.Backend.Infrastructure.Tests.Unit.GoCardless;

/// <summary>
///     Tests to ensure all DTOs correctly map to the GoCardless API specification.
/// </summary>
public class DtoMappingTests
{
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

    [Fact]
    public void AccountDto_ShouldDeserializeCorrectly()
    {
        // Arrange - Sample response from GoCardless API specification
        const string json = """
                            {
                                "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                                "created": "2023-08-03T10:30:00Z",
                                "last_accessed": "2023-08-03T12:00:00Z",
                                "iban": "GB29NWBK60161331926819",
                                "bban": "NWBK60161331926819",
                                "status": "READY",
                                "institution_id": "SANDBOXFINANCE_SFIN0000",
                                "owner_name": "John Doe",
                                "name": "Main Account"
                            }
                            """;

        // Act
        GetAccountResponseDto? result = JsonSerializer.Deserialize<GetAccountResponseDto>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("3fa85f64-5717-4562-b3fc-2c963f66afa6", result.Id);
        Assert.Equal(DateTime.Parse("2023-08-03T10:30:00Z").ToUniversalTime(), result.Created.ToUniversalTime());
        Assert.Equal(DateTime.Parse("2023-08-03T12:00:00Z").ToUniversalTime(), result.LastAccessed.ToUniversalTime());
        Assert.Equal("GB29NWBK60161331926819", result.Iban);
        Assert.Equal("NWBK60161331926819", result.Bban);
        Assert.Equal("READY", result.Status);
        Assert.Equal("SANDBOXFINANCE_SFIN0000", result.InstitutionId);
        Assert.Equal("John Doe", result.OwnerName);
        Assert.Equal("Main Account", result.Name);
    }
}
