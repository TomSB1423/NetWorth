namespace Networth.Functions.Options;

/// <summary>
///     Configuration options for the mock user in development.
/// </summary>
public class MockUserOptions
{
    /// <summary>Gets or sets the mock Firebase UID.</summary>
    public string? FirebaseUid { get; set; }

    /// <summary>Gets or sets the mock user's display name.</summary>
    public string? Name { get; set; }

    /// <summary>Gets or sets the mock user's email.</summary>
    public string? Email { get; set; }
}
