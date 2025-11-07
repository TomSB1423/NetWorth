using Networth.Infrastructure.Gocardless.Options;

namespace Networth.Functions.Tests.Integration.Infrastructure;

/// <summary>
///     Configuration keys for GoCardless settings.
/// </summary>
public static class GoCardlessConfiguration
{
    /// <summary>
    ///     Gets the test secret ID value.
    /// </summary>
    public const string TestSecretId = "test-secret-id";

    /// <summary>
    ///     Gets the test secret key value.
    /// </summary>
    public const string TestSecretKey = "test-secret-key";

    /// <summary>
    ///     Gets the configuration section name for GoCardless.
    /// </summary>
    public const string SectionName = "Gocardless";

    /// <summary>
    ///     Gets the configuration key for the GoCardless base URL.
    /// </summary>
    public static string BankAccountDataBaseUrl => $"{SectionName}:{nameof(GocardlessOptions.BankAccountDataBaseUrl)}";

    /// <summary>
    ///     Gets the configuration key for the GoCardless secret ID.
    /// </summary>
    public static string SecretId => $"{SectionName}:{nameof(GocardlessOptions.SecretId)}";

    /// <summary>
    ///     Gets the configuration key for the GoCardless secret key.
    /// </summary>
    public static string SecretKey => $"{SectionName}:{nameof(GocardlessOptions.SecretKey)}";
}
