namespace Networth.Infrastructure.Data.Entities;

/// <summary>
///     Represents a user's connection to a financial institution.
/// </summary>
public class Institution
{
    /// <summary>
    ///     Gets or sets the unique identifier for the user-institution link.
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
    public required string GoCardlessInstitutionId { get; set; }

    /// <summary>
    ///     Gets or sets the requisition ID.
    /// </summary>
    public required string RequisitionId { get; set; }

    /// <summary>
    ///     Gets or sets the requisition.
    /// </summary>
    public Requisition Requisition { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the accounts associated with this user-institution connection.
    /// </summary>
    public ICollection<Account> Accounts { get; set; } = [];
}
