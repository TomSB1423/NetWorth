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
}
