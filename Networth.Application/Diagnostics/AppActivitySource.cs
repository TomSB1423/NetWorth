using System.Diagnostics;

namespace Networth.Application.Diagnostics;

/// <summary>
///     Provides the <see cref="ActivitySource"/> for application-level distributed tracing.
/// </summary>
public static class AppActivitySource
{
    /// <summary>
    ///     The name of the activity source used for tracing.
    /// </summary>
    public const string Name = "Networth.Application";

    /// <summary>
    ///     Gets the <see cref="ActivitySource"/> instance for creating activities.
    /// </summary>
    public static readonly ActivitySource Instance = new(Name);
}
