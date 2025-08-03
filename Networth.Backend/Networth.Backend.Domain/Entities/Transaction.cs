namespace Networth.Backend.Domain.Entities;

/// <summary>
/// Represents a transaction within a bank account.
/// </summary>
public record Transaction
{
    /// <summary>
    /// Gets the unique identifier for the transaction.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the amount of the transaction.
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Gets the description of the transaction.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the date when the transaction occurred.
    /// </summary>
    public DateTime Date { get; init; }

    /// <summary>
    /// Gets the category of the transaction.
    /// </summary>
    public string? Category { get; init; }
}
