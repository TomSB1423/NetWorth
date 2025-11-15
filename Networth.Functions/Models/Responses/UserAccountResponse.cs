namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for a user's bank account.
/// </summary>
public record UserAccountResponse
{
    /// <summary>
    ///     Gets the unique identifier for the account.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     Gets the user ID.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    ///     Gets the requisition ID.
    /// </summary>
    public required string RequisitionId { get; init; }

    /// <summary>
    ///     Gets the institution metadata ID.
    /// </summary>
    public required string InstitutionId { get; init; }

    /// <summary>
    ///     Gets the name of the account.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Gets the IBAN of the account.
    /// </summary>
    public string? Iban { get; init; }

    /// <summary>
    ///     Gets the currency of the account.
    /// </summary>
    public required string Currency { get; init; }

    /// <summary>
    ///     Gets the product name/type.
    /// </summary>
    public string? Product { get; init; }
}
