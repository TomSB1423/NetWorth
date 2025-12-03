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
    ///     Gets a value indicating whether to include sandbox institutions (development only).
    /// </summary>
    public bool IncludeSandbox { get; init; }
}
