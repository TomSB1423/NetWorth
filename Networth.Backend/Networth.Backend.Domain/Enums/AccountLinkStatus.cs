using System.Text.Json.Serialization;

namespace Networth.Backend.Domain.Enums;

/// <summary>
///     Represents the status of an account link.
/// </summary>
public enum AccountLinkStatus
{
    /// <summary>
    ///     The account is successfully linked.
    /// </summary>
    Linked = 0,

    /// <summary>
    ///     The linking failed.
    /// </summary>
    Failed,

    /// <summary>
    ///     The linking process is pending.
    /// </summary>
    Pending,

    /// <summary>
    ///     The account expired.
    /// </summary>
    Expired,
}
