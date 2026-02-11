using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Application.Models;
using Networth.Application.Queries;
using Networth.Domain.Repositories;

namespace Networth.Application.Handlers;

/// <summary>
///     Handler for GetTransactionsQuery.
/// </summary>
public class GetTransactionsQueryHandler(
    ITransactionRepository transactionRepository,
    ILogger<GetTransactionsQueryHandler> logger)
    : IRequestHandler<GetTransactionsQuery, GetTransactionsQueryResult>
{
    /// <inheritdoc />
    public async Task<GetTransactionsQueryResult> HandleAsync(
        GetTransactionsQuery query,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Getting paginated transactions for account: {AccountId}, page: {Page}, pageSize: {PageSize}",
            query.AccountId,
            query.Page,
            query.PageSize);

        var (items, totalCount) = await transactionRepository.GetByAccountIdPagedAsync(
            query.AccountId,
            query.Page,
            query.PageSize,
            cancellationToken);

        var pagedResult = new PagedResult<Domain.Entities.Transaction>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
        };

        logger.LogInformation(
            "Retrieved page {Page} of {TotalPages} ({Count} items) for account {AccountId}",
            pagedResult.Page,
            pagedResult.TotalPages,
            pagedResult.Items.Count(),
            query.AccountId);

        return new GetTransactionsQueryResult
        {
            AccountId = query.AccountId,
            Transactions = pagedResult,
        };
    }
}
