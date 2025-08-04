namespace Networth.Backend.Domain.Entities;

/// <summary>
///     Represents detailed account information.
/// </summary>
public class AccountDetail
{
    /// <summary>
    ///     Gets or sets the resource ID.
    /// </summary>
    public string? ResourceId { get; set; }

    /// <summary>
    ///     Gets or sets the IBAN.
    /// </summary>
    public string? Iban { get; set; }

    /// <summary>
    ///     Gets or sets the BBAN.
    /// </summary>
    public string? Bban { get; set; }

    /// <summary>
    ///     Gets or sets the SortCodeAccountNumber returned by some UK banks.
    /// </summary>
    public string? Scan { get; set; }

    /// <summary>
    ///     Gets or sets the MSISDN.
    /// </summary>
    public string? Msisdn { get; set; }

    /// <summary>
    ///     Gets or sets the currency.
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    ///     Gets or sets the owner name.
    /// </summary>
    public string? OwnerName { get; set; }

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

    /// <summary>
    ///     Gets or sets the BIC.
    /// </summary>
    public string? Bic { get; set; }

    /// <summary>
    ///     Gets or sets the linked accounts.
    /// </summary>
    public string? LinkedAccounts { get; set; }

    /// <summary>
    ///     Gets or sets the masked PAN.
    /// </summary>
    public string? MaskedPan { get; set; }

    /// <summary>
    ///     Gets or sets the usage.
    /// </summary>
    public string? Usage { get; set; }

    /// <summary>
    ///     Gets or sets the details.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    ///     Gets or sets the owner address unstructured.
    /// </summary>
    public string[]? OwnerAddressUnstructured { get; set; }

    /// <summary>
    ///     Gets or sets the owner address structured.
    /// </summary>
    public OwnerAddressStructured? OwnerAddressStructured { get; set; }

    /// <summary>
    ///     Gets or sets additional account data.
    /// </summary>
    public string? AdditionalAccountData { get; set; }
}
