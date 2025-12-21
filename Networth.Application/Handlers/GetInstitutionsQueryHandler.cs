using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Queries;
using Networth.Domain.Entities;
using Networth.Domain.Enums;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for GetInstitutionsQuery.
/// </summary>
public class GetInstitutionsQueryHandler(
    IFinancialProvider financialProvider,
    ICacheMetadataRepository cacheMetadataRepository,
    IInstitutionMetadataRepository institutionMetadataRepository,
    IRequisitionRepository requisitionRepository,
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

        string cacheKey = $"institutions_{query.CountryCode}";

        // Check if cache is fresh (less than 30 days old)
        bool isCacheFresh = await cacheMetadataRepository.IsCacheFreshAsync(cacheKey, CacheMaxAgeDays, cancellationToken);

        GetInstitutionsQueryResult result;
        if (isCacheFresh)
        {
            result = await GetCachedInstitutionsAsync(query, cancellationToken);
        }
        else
        {
            result = await FetchAndCacheInstitutionsAsync(query, cacheKey, cancellationToken);
        }

        // Filter out linked institutions if requested
        if (query.ExcludeLinked && query.UserId.HasValue)
        {
            result = await FilterOutLinkedInstitutionsAsync(result, query.UserId.Value, cancellationToken);
        }

        return result;
    }

    private async Task<GetInstitutionsQueryResult> FilterOutLinkedInstitutionsAsync(
        GetInstitutionsQueryResult result,
        Guid userId,
        CancellationToken cancellationToken)
    {
        // Get all linked institution IDs for this user
        var linkedInstitutionIds = await requisitionRepository.GetLinkedInstitutionIdsForUserAsync(userId, cancellationToken);
        var linkedSet = linkedInstitutionIds.ToHashSet();

        if (linkedSet.Count == 0)
        {
            return result;
        }

        logger.LogInformation(
            "Filtering out {Count} already linked institutions for user {UserId}",
            linkedSet.Count,
            userId);

        var filteredInstitutions = result.Institutions
            .Where(i => !linkedSet.Contains(i.Id))
            .ToList();

        return new GetInstitutionsQueryResult
        {
            Institutions = filteredInstitutions,
        };
    }

    private async Task<GetInstitutionsQueryResult> GetCachedInstitutionsAsync(GetInstitutionsQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Cache is fresh for country {CountryCode}, retrieving from database", query.CountryCode);

        IEnumerable<InstitutionMetadata> cachedInstitutions =
            await institutionMetadataRepository.GetByCountryAsync(query.CountryCode, cancellationToken);
        List<InstitutionMetadata> cachedList = cachedInstitutions.ToList();

        return new GetInstitutionsQueryResult
        {
            Institutions = cachedList,
        };
    }

    private async Task<GetInstitutionsQueryResult> FetchAndCacheInstitutionsAsync(
        GetInstitutionsQuery query,
        string cacheKey,
        CancellationToken cancellationToken)
    {
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
