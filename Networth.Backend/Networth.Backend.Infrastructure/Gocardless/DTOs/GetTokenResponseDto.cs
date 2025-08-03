namespace Networth.Backend.Infrastructure.Gocardless.DTOs;

internal class GetTokenResponseDto
{
    public required string Access { get; set; }

    public int? AccessExpires { get; set; }
}
