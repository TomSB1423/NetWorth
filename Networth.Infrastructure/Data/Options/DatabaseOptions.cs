namespace Networth.Infrastructure.Data.Options;

/// <summary>
///     Configuration options for database connection and settings.
/// </summary>
public class DatabaseOptions
{
    /// <summary>
    ///     Gets the database connection string.
    /// </summary>
    public string ConnectionString { get; init; } = string.Empty;
}
