namespace Networth.Infrastructure.Gocardless.Options;

public class GocardlessOptions
{
    /// <summary>
    ///     Gets or sets the base URL for the GoCardless Bank Account Data API.
    /// </summary>
    public string BankAccountDataBaseUrl { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the secret ID for GoCardless API authentication.
    /// </summary>
    public string SecretId { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the secret key for GoCardless API authentication.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
}
