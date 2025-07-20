namespace Networth.Backend.Domain.Entities;

/// <summary>
/// Represents a bank institution with its associated accounts.
/// </summary>
public record Institution
{
    /// <summary>
    /// Gets the unique identifier for the institution.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Gets the name of the bank.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the unique identifier for the institution.
    /// </summary>
    public required string InstitutionId { get; init; }

    /// <summary>
    /// Gets the list of bank accounts associated with this bank.
    /// </summary>
    public IReadOnlyList<Account> Accounts { get; init; } = new List<Account>();
}
