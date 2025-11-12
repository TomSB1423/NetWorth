using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for GetInstitutionsQuery.
/// </summary>
public class GetInstitutionsQueryHandler(
    IFinancialProvider financialProvider,
    ICacheMetadataRepository cacheMetadataRepository,
    IInstitutionMetadataRepository institutionMetadataRepository,
    ILogger<GetInstitutionsQueryHandler> logger)
    : IRequestHandler<GetInstitutionsQuery, GetInstitutionsQueryResult>
{
    private const int CacheMaxAgeDays = 30;

    /// <inheritdoc />
    public async Task<GetInstitutionsQueryResult> HandleAsync(
        GetInstitutionsQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving institutions for country {CountryCode}", query.CountryCode);

        var cacheKey = $"institutions_{query.CountryCode}";

        // Check if cache is fresh (less than 30 days old)
        bool isCacheFresh = await cacheMetadataRepository.IsCacheFreshAsync(cacheKey, CacheMaxAgeDays, cancellationToken);

        if (isCacheFresh)
        {
            logger.LogInformation("Cache is fresh for country {CountryCode}, retrieving from database", query.CountryCode);

            var cachedInstitutions = await institutionMetadataRepository.GetByCountryAsync(query.CountryCode, cancellationToken);

            return new GetInstitutionsQueryResult
            {
                Institutions = cachedInstitutions,
            };
        }

        // Cache is stale or doesn't exist, fetch from GoCardless
        logger.LogInformation("Cache is stale for country {CountryCode}, fetching from GoCardless", query.CountryCode);

        var institutions = await financialProvider.GetInstitutionsAsync(query.CountryCode, cancellationToken);
        var institutionsList = institutions.ToList();

        // Save to database
        await institutionMetadataRepository.SaveInstitutionsAsync(query.CountryCode, institutionsList, cancellationToken);

        // Update cache metadata
        await cacheMetadataRepository.UpsertAsync(cacheKey, institutionsList.Count, cancellationToken);

        logger.LogInformation("Successfully refreshed cache for country {CountryCode} with {Count} institutions", query.CountryCode, institutionsList.Count);

        return new GetInstitutionsQueryResult
        {
            Institutions = institutionsList,
        };
    }
}
