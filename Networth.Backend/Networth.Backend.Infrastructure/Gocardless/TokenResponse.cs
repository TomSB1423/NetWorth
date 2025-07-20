namespace Networth.Backend.Infrastructure.Gocardless;

internal class TokenResponse
{
    public required string Access { get; set; }

    public int? AccessExpires { get; set; }
}
