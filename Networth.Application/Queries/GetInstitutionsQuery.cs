using Networth.Application.Interfaces;

namespace Networth.Application.Queries;

/// <summary>
///     Query for retrieving institutions.
/// </summary>
public class GetInstitutionsQuery : IRequest<GetInstitutionsQueryResult>
{
    /// <summary>
    ///     Gets the country code.
    /// </summary>
    public required string CountryCode { get; init; }

    /// <summary>
    ///     Gets the internal user ID to filter out already linked institutions.
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    ///     Gets a value indicating whether to exclude institutions the user has already linked.
    /// </summary>
    public bool ExcludeLinked { get; init; }
}
