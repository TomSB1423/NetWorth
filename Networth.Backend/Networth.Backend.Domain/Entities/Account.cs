using System.ComponentModel.DataAnnotations;

namespace Networth.Backend.Domain.Entities;

/// <summary>
/// Represents a bank account with its transactions and balance information.
/// </summary>
public record Account
{
    /// <summary>
    /// Gets the unique identifier for the bank account.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the name of the bank account.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the current balance of the bank account.
    /// </summary>
    public decimal Balance { get; init; }

    /// <summary>
    /// Gets the list of transactions for the bank account.
    /// </summary>
    public IReadOnlyList<Transaction> Transactions { get; init; } = new List<Transaction>();

    /// <summary>
    /// Gets the currency of the bank account balance.
    /// </summary>
    public required string Currency { get; init; }

    /// <summary>
    /// Gets the type of the bank account.
    /// </summary>
    public required BankAccountType Type { get; init; }
}
