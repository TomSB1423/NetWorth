namespace Networth.Backend.Domain.Entities;

/// <summary>
///     Represents a bank institution with its associated accounts.
/// </summary>
public record Institution
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
    ///     Gets the list of bank accounts associated with this bank.
    /// </summary>
    public IReadOnlyList<Account> Accounts { get; init; } = new List<Account>();
}
