namespace Networth.Functions.Authentication;

/// <summary>
///     The default mock user for development.
///     Uses obvious "mock-" prefix to distinguish from real Firebase UIDs.
/// </summary>
internal static class MockUser
{
    /// <summary>The mock Firebase UID.</summary>
    public const string FirebaseUid = "mock-user-123";

    /// <summary>The mock user's display name.</summary>
    public const string Name = "Mock Development User";

    /// <summary>The mock user's email.</summary>
    public const string Email = "mock@example.com";
}
