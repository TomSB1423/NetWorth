using Networth.Domain.Entities;

namespace Networth.Application.Queries;

/// <summary>
///     Result for GetInstitutionsQuery.
/// </summary>
public class GetInstitutionsQueryResult
{
    /// <summary>
    ///     Gets the list of institutions.
    /// </summary>
    public required IEnumerable<Institution> Institutions { get; init; }
}
