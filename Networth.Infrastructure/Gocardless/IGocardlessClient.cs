using Networth.Infrastructure.Gocardless.DTOs;
using Refit;

namespace Networth.Infrastructure.Gocardless;

/// <summary>
///     Defines the interface for interacting with the GoCardless Bank Account Data API.
/// </summary>
internal interface IGocardlessClient
{
    [Post("/token/new/")]
    Task<ApiResponse<GetTokenResponseDto>> GetAccessTokenAsync([Body] TokenRequestDto requestDto);

    [Get("/institutions/{id}/")]
    [Headers("Authorization: Bearer")]
    Task<ApiResponse<GetInstitutionDto>> GetInstitution(string id, CancellationToken cancellationToken = default);

    [Get("/institutions/")]
    [Headers("Authorization: Bearer")]
    Task<ApiResponse<IEnumerable<GetInstitutionDto>>> GetInstitutions([Query("country")] string country, CancellationToken cancellationToken = default);

    [Post("/agreements/enduser/")]
    [Headers("Authorization: Bearer")]
    Task<ApiResponse<CreateAgreementResponseDto>> CreateAgreement([Body] CreateAgreementRequestDto agreement, CancellationToken cancellationToken = default);

    [Post("/requisitions/")]
    [Headers("Authorization: Bearer")]
    Task<ApiResponse<CreateRequisitionResponseDto>> CreateRequisition(
        [Body] CreateRequisitionRequestDto requisition,
        CancellationToken cancellationToken = default);

    [Get("/requisitions/{id}/")]
    [Headers("Authorization: Bearer")]
    Task<ApiResponse<GetRequisitionResponseDto>> GetRequisition(string id, CancellationToken cancellationToken = default);

    [Get("/accounts/{id}/")]
    [Headers("Authorization: Bearer")]
    Task<ApiResponse<GetAccountResponseDto>> GetAccount(string id, CancellationToken cancellationToken = default);

    [Get("/accounts/{id}/balances/")]
    [Headers("Authorization: Bearer")]
    Task<ApiResponse<GetAccountBalanceResponseDto>> GetAccountBalances(string id, CancellationToken cancellationToken = default);

    [Get("/accounts/{id}/details/")]
    [Headers("Authorization: Bearer")]
    Task<ApiResponse<GetAccountDetailResponseDto>> GetAccountDetails(string id, CancellationToken cancellationToken = default);

    [Get("/accounts/{id}/transactions/")]
    [Headers("Authorization: Bearer")]
    Task<ApiResponse<GetAccountTransactionsResponseDto>> GetAccountTransactions(
        string id,
        [Query("date_from")] string? dateFrom = null,
        [Query("date_to")] string? dateTo = null,
        CancellationToken cancellationToken = default);
}
