using Networth.Domain.Enums;

namespace Networth.Functions.Models.Responses;

/// <summary>
///     Response model for linking a bank account.
/// </summary>
public record LinkAccountResponse
{
    /// <summary>
    ///     Gets the authorization link for the user to complete bank authentication.
    /// </summary>
    public required string AuthorizationLink { get; init; }

    /// <summary>
    ///     Gets the status of the link.
    /// </summary>
    public required AccountLinkStatus Status { get; init; }
}
