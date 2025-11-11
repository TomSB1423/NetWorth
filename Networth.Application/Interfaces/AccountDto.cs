namespace Networth.Application.Interfaces;

/// <summary>
///     Data transfer object for account information from the database.
/// </summary>
public class AccountDto
{
    /// <summary>
    ///     Gets the unique identifier for the account.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     Gets the institution identifier.
    /// </summary>
    public required string InstitutionId { get; init; }

    /// <summary>
    ///     Gets the name of the account.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Gets the institution information.
    /// </summary>
    public InstitutionInfo? Institution { get; init; }
}
