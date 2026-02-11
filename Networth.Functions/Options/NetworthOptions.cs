namespace Networth.Functions.Options;

/// <summary>
///     Configuration options for the Networth application.
/// </summary>
public class NetworthOptions
{
    /// <summary>The configuration section name.</summary>
    public const string SectionName = "Networth";

    /// <summary>
    ///     Gets or sets a value indicating whether to use real authentication.
    ///     When false, mock authentication with the configured mock user is used.
    /// </summary>
    public bool UseAuthentication { get; set; }

    /// <summary>
    ///     Gets or sets the mock user configuration.
    ///     Used when <see cref="UseAuthentication" /> is false.
    /// </summary>
    public MockUserOptions MockUser { get; set; } = new();
}
