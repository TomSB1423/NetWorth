using Networth.Backend.Domain.Entities;

namespace Networth.Backend.Application.Interfaces;

/// <summary>
///     Interface for financial provider services that handles all GoCardless operations.
/// </summary>
public interface IFinancialProvider
{
    /// <summary>
    ///     Gets a list of available institutions for a given country.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of available institutions.</returns>
    Task<IEnumerable<Institution>> GetInstitutionsAsync(CancellationToken cancellationToken = default);

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
        int maxHistoricalDays = 90,
        int accessValidForDays = 90,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a requisition for connecting to a bank account.
    /// </summary>
    /// <param name="redirectUrl">The URL to redirect to after authentication.</param>
    /// <param name="institutionId">The ID of the financial institution.</param>
    /// <param name="agreementId">The agreement ID for the connection.</param>
    /// <param name="reference">A reference identifier for the requisition.</param>
    /// <param name="userLanguage">The user's preferred language (default: EN).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created requisition with authorization link.</returns>
    Task<Requisition> CreateRequisitionAsync(
        string redirectUrl,
        string institutionId,
        string agreementId,
        string reference,
        string userLanguage = "EN",
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a requisition by its ID.
    /// </summary>
    /// <param name="requisitionId">The requisition ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requisition details.</returns>
    Task<Requisition> GetRequisitionAsync(string requisitionId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets account metadata by account ID.
    /// </summary>
    /// <param name="accountId">The account ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The account metadata.</returns>
    Task<Account> GetAccountAsync(string accountId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets account balances by account ID.
    /// </summary>
    /// <param name="accountId">The account ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The account balances.</returns>
    Task<IEnumerable<AccountBalance>> GetAccountBalancesAsync(
        string accountId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets account details by account ID.
    /// </summary>
    /// <param name="accountId">The account ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The account details.</returns>
    Task<AccountDetail> GetAccountDetailsAsync(string accountId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets account transactions by account ID.
    /// </summary>
    /// <param name="accountId">The account ID.</param>
    /// <param name="dateFrom">Start date for transaction filtering (optional).</param>
    /// <param name="dateTo">End date for transaction filtering (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The account transactions.</returns>
    Task<IEnumerable<Transaction>> GetAccountTransactionsAsync(
        string accountId,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default);
}
