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
    [JsonPropertyName("linked")]
    Linked = 0,

    /// <summary>
    ///     The linking failed.
    /// </summary>
    [JsonPropertyName("failed")]
    Failed,

    /// <summary>
    ///     The linking process is pending.
    /// </summary>
    [JsonPropertyName("pending")]
    Pending,

    /// <summary>
    ///     The account expired.
    /// </summary>
    [JsonPropertyName("expired")]
    Expired,
}
