using System.Text.Json.Serialization;

namespace Networth.Domain.Enums;

/// <summary>
///     Represents the status of an account link requisition.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountLinkStatus
{
    /// <summary>
    ///     The requisition is pending or in progress.
    /// </summary>
    Pending = 0,

    /// <summary>
    ///     The account is successfully linked.
    /// </summary>
    Linked,

    /// <summary>
    ///     The requisition failed or was rejected.
    /// </summary>
    Failed,

    /// <summary>
    ///     The requisition has expired.
    /// </summary>
    Expired,
}
