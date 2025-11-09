namespace Networth.Domain.Entities;

/// <summary>
///     Represents detailed account information.
/// </summary>
public class AccountDetail
{
    /// <summary>
    ///     Gets or sets the resource ID.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    ///     Gets or sets the currency.
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    ///     Gets or sets the account name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets the display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    ///     Gets or sets the product.
    /// </summary>
    public string? Product { get; set; }

    /// <summary>
    ///     Gets or sets the cash account type.
    /// </summary>
    public string? CashAccountType { get; set; }

    /// <summary>
    ///     Gets or sets the status.
    /// </summary>
    public string? Status { get; set; }
}
