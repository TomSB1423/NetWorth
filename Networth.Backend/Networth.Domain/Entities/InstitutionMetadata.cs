namespace Networth.Backend.Domain.Entities;

/// <summary>
///     Represents metadata about a financial institution from GoCardless.
///     This is a read-only DTO and not stored in the database.
/// </summary>
public record InstitutionMetadata
{
    /// <summary>
    ///     Gets the unique identifier for the institution.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     Gets the name of the bank.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Gets the transaction history available in days.
    /// </summary>
    public int? TransactionTotalDays { get; init; }

    /// <summary>
    ///     Gets the maximum number of days the authorization is valid for.
    /// </summary>
    public int? MaxAccessValidForDays { get; init; }

    /// <summary>
    ///     Gets the logo URL of the bank institution.
    /// </summary>
    public string? LogoUrl { get; init; }

    /// <summary>
    ///     Gets the BIC code of the institution.
    /// </summary>
    public string? Bic { get; init; }

    /// <summary>
    ///     Gets the countries where this institution operates.
    /// </summary>
    public string[] Countries { get; init; } = [];
}

