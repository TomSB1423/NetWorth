namespace Networth.ServiceDefaults;

/// <summary>
///     Constants for Aspire resource names used throughout the application.
/// </summary>
public static class ResourceNames
{
    /// <summary>
    ///     Gets the resource name for the PostgreSQL server.
    /// </summary>
    public const string Postgres = "postgres";

    /// <summary>
    ///     Gets the resource name for the NetWorth database.
    /// </summary>
    public const string NetworthDb = "networth-db";

    /// <summary>
    ///     Gets the resource name for the Azure Functions backend.
    /// </summary>
    public const string Functions = "functions";

    /// <summary>
    ///     Gets the resource name for the React frontend.
    /// </summary>
    public const string React = "react";

    /// <summary>
    ///     Gets the resource name for the Azure Storage account.
    /// </summary>
    public const string Storage = "storage";

    /// <summary>
    ///     Gets the resource name for the Azure Storage Queues.
    /// </summary>
    public const string Queues = "queues";

    /// <summary>
    ///     Gets the resource name for the Docusaurus documentation site.
    /// </summary>
    public const string Docs = "docs";

    /// <summary>
    ///     Gets the queue name for account sync operations.
    /// </summary>
    public const string AccountSyncQueue = "account-sync";

    /// <summary>
    ///     Gets the queue name for calculate running balance operations.
    /// </summary>
    public const string CalculateRunningBalanceQueue = "calculate-running-balance";
}

