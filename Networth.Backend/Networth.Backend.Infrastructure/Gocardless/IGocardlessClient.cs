using Networth.Backend.Infrastructure.Gocardless.DTOs;
using Refit;

namespace Networth.Backend.Infrastructure.Gocardless;

/// <summary>
///     Defines the interface for interacting with the GoCardless Bank Account Data API.
/// </summary>
internal interface IGocardlessClient
{
    [Post("/token/new/")]
    Task<GetTokenResponseDto> GetAccessTokenAsync([Body] TokenRequestDto requestDto);

    [Get("/institutions/{id}/")]
    [Headers("Authorization: Bearer")]
    Task<GetInstitutionDto> GetInstitution(string id, CancellationToken cancellationToken = default);

    [Get("/institutions/")]
    [Headers("Authorization: Bearer")]
    Task<IEnumerable<GetInstitutionDto>> GetInstitutions([Query("country")] string country, CancellationToken cancellationToken = default);

    [Post("/agreements/enduser/")]
    [Headers("Authorization: Bearer")]
    Task<CreateAgreementResponseDto> CreateAgreement([Body] CreateAgreementRequestDto agreement, CancellationToken cancellationToken = default);

    [Post("/requisitions/")]
    [Headers("Authorization: Bearer")]
    Task<CreateRequisitionResponseDto> CreateRequisition(
        [Body] CreateRequisitionRequestDto requisition,
        CancellationToken cancellationToken = default);

    [Get("/requisitions/{id}/")]
    [Headers("Authorization: Bearer")]
    Task<GetRequisitionResponseDto> GetRequisition(string id, CancellationToken cancellationToken = default);

    [Get("/accounts/{id}/")]
    [Headers("Authorization: Bearer")]
    Task<GetAccountResponseDto> GetAccount(string id, CancellationToken cancellationToken = default);

    [Get("/accounts/{id}/balances/")]
    [Headers("Authorization: Bearer")]
    Task<GetAccountBalanceResponseDto> GetAccountBalances(string id, CancellationToken cancellationToken = default);

    [Get("/accounts/{id}/details/")]
    [Headers("Authorization: Bearer")]
    Task<GetAccountDetailResponseDto> GetAccountDetails(string id, CancellationToken cancellationToken = default);

    [Get("/accounts/{id}/transactions/")]
    [Headers("Authorization: Bearer")]
    Task<GetAccountTransactionsResponseDto> GetAccountTransactions(
        string id,
        [Query("date_from")] string? dateFrom = null,
        [Query("date_to")] string? dateTo = null,
        CancellationToken cancellationToken = default);
}
