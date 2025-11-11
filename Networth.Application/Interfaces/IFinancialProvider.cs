using Networth.Domain.Entities;

namespace Networth.Application.Interfaces;

/// <summary>
///     Interface for financial provider services that handles all GoCardless operations.
/// </summary>
public interface IFinancialProvider
{
    /// <summary>
    ///     Gets a single institution's metadata.
    /// </summary>
    /// <param name="institutionId">The institution id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Institution metadata from GoCardless.</returns>
    Task<Institution> GetInstitutionAsync(string institutionId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a list of available institutions for a given country.
    /// </summary>
    /// <param name="country">The institution country.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of available institution metadata.</returns>
    Task<IEnumerable<Institution>> GetInstitutionsAsync(string country, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates an agreement for accessing bank account data.
    /// </summary>
    /// <param name="institutionId">The ID of the financial institution.</param>
    /// <param name="maxHistoricalDays">Maximum number of days of historical data to access (default: 90).</param>
    /// <param name="accessValidForDays">Number of days the access token is valid (default: 90).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created agreement.</returns>
    Task<Agreement> CreateAgreementAsync(
        string institutionId,
        int? maxHistoricalDays,
        int? accessValidForDays,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a requisition for connecting to a bank account.
    /// </summary>
    /// <param name="institutionId">The ID of the financial institution.</param>
    /// <param name="agreementId">The agreement ID for the connection.</param>
    /// <param name="redirectUrl">The URL to redirect to after authentication.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created requisition with authorization link.</returns>
    Task<Requisition> CreateRequisitionAsync(
        string institutionId,
        string agreementId,
        string redirectUrl,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a requisition by its ID.
    /// </summary>
    /// <param name="requisitionId">The requisition ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requisition details, or null if not found.</returns>
    Task<Requisition?> GetRequisitionAsync(string requisitionId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets account metadata by account ID.
    /// </summary>
    /// <param name="accountId">The account ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The account metadata from GoCardless, or null if not found.</returns>
    Task<Account?> GetAccountAsync(string accountId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets account balances by account ID.
    /// </summary>
    /// <param name="accountId">The account ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The account balances, or null if not found.</returns>
    Task<IEnumerable<AccountBalance>?> GetAccountBalancesAsync(
        string accountId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets account details by account ID.
    /// </summary>
    /// <param name="accountId">The account ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The account details, or null if not found.</returns>
    Task<AccountDetail?> GetAccountDetailsAsync(string accountId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets account transactions by account ID.
    /// </summary>
    /// <param name="accountId">The account ID.</param>
    /// <param name="dateFrom">Start date for transaction filtering (optional).</param>
    /// <param name="dateTo">End date for transaction filtering (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The account transactions from GoCardless, or null if not found.</returns>
    Task<IEnumerable<Transaction>?> GetAccountTransactionsAsync(
        string accountId,
        DateTimeOffset dateFrom,
        DateTimeOffset dateTo,
        CancellationToken cancellationToken = default);
}
