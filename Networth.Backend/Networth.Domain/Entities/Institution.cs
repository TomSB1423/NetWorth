namespace Networth.Backend.Domain.Entities;

/// <summary>
///     Represents a financial institution connected through GoCardless.
/// </summary>
public class Institution
{
    /// <summary>
    ///     Gets or sets the unique identifier for the institution.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    ///     Gets or sets the owner user ID.
    /// </summary>
    public required string OwnerId { get; set; }

    /// <summary>
    ///     Gets or sets the owner user.
    /// </summary>
    public User Owner { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the GoCardless institution ID.
    /// </summary>
    public required string GoCardlessId { get; set; }

    /// <summary>
    ///     Gets or sets the requisition ID.
    /// </summary>
    public required string RequisitionId { get; set; }

    /// <summary>
    ///     Gets or sets the requisition.
    /// </summary>
    public Requisition Requisition { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the accounts associated with this institution.
    /// </summary>
    public ICollection<Account> Accounts { get; set; } = [];
}
