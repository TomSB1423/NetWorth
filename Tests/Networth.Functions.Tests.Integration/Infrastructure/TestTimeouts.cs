namespace Networth.Backend.Functions.Tests.Integration.Infrastructure;

/// <summary>
///     Configuration for test timeouts.
/// </summary>
public static class TestTimeouts
{
    /// <summary>
    ///     Gets the default test timeout.
    /// </summary>
    public static readonly TimeSpan Default = TimeSpan.FromMinutes(5);

    /// <summary>
    ///     Gets the short timeout for quick operations.
    /// </summary>
    public static readonly TimeSpan Short = TimeSpan.FromMinutes(2);
}
