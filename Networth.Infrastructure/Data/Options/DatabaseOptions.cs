namespace Networth.Infrastructure.Data.Options;

/// <summary>
///     Configuration options for database connection and settings.
/// </summary>
public class DatabaseOptions
{
    /// <summary>
    ///     The configuration section name.
    /// </summary>
    public const string SectionName = "";

    /// <summary>
    ///     Gets the database connection string.
    /// </summary>
    public string ConnectionString { get; init; } = string.Empty;

    /// <summary>
    ///     Gets a value indicating whether to automatically run migrations on startup.
    /// </summary>
    public bool AutoMigrate { get; init; }
}
