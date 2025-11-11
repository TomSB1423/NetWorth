using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for GetInstitutionsQuery.
/// </summary>
public class GetInstitutionsQueryHandler(
    IFinancialProvider financialProvider,
    ILogger<GetInstitutionsQueryHandler> logger)
    : IRequestHandler<GetInstitutionsQuery, GetInstitutionsQueryResult>
{
    /// <inheritdoc />
    public async Task<GetInstitutionsQueryResult> HandleAsync(
        GetInstitutionsQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving institutions for country {CountryCode}", query.CountryCode);

        var institutions = await financialProvider.GetInstitutionsAsync(query.CountryCode, cancellationToken);

        logger.LogInformation("Successfully retrieved institutions for country {CountryCode}", query.CountryCode);

        return new GetInstitutionsQueryResult
        {
            Institutions = institutions,
        };
    }
}
