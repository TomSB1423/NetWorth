namespace Networth.Application.Options;

/// <summary>
///     Options for institution data source configuration.
/// </summary>
public class InstitutionsOptions
{
    public const string SectionName = "Institutions";

    /// <summary>
    ///     Gets or sets a value indicating whether to use the sandbox institution table.
    ///     When true, reads from SandboxInstitution table and disables API sync.
    ///     When false (default), reads from Institutions table with normal API sync.
    /// </summary>
    public bool UseSandbox { get; set; }
}
