namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for detailed account information.
/// </summary>
public class AccountDetailResponse
{
    /// <summary>
    ///     Gets the resource ID.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    ///     Gets the currency.
    /// </summary>
    public string? Currency { get; init; }

    /// <summary>
    ///     Gets the account name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     Gets the display name.
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    ///     Gets the product.
    /// </summary>
    public string? Product { get; init; }

    /// <summary>
    ///     Gets the cash account type.
    /// </summary>
    public string? CashAccountType { get; init; }

    /// <summary>
    ///     Gets the status.
    /// </summary>
    public string? Status { get; init; }
}
