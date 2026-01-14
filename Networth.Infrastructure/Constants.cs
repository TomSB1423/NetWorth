namespace Networth.Infrastructure;

internal static class Constants
{
    /// <summary>
    ///     The well-known sandbox institution ID from GoCardless.
    /// </summary>
    public const string SandboxInstitutionId = "SANDBOXFINANCE_SFIN0000";

    /// <summary>
    ///     Constants for option configuration Names.
    /// </summary>
    public static class OptionsNames
    {
        public const string GocardlessSection = "Gocardless";
    }

    /// <summary>
    ///     Table names for database entities.
    /// </summary>
    public static class TableNames
    {
        public const string Institutions = "Institutions";
        public const string SandboxInstitution = "SandboxInstitution";
    }
}
