using Networth.Backend.Infrastructure.Gocardless.Entities;
using Refit;

namespace Networth.Backend.Infrastructure.Gocardless;

internal interface IGocardlessClient
{
    [Post("/token/new/")]
    Task<TokenResponse> GetAccessTokenAsync([Body] TokenRequest request);

    [Get("/institutions/")]
    [Headers("Authorization: Bearer")]
    Task<IEnumerable<InstitutionDto>> GetInstitutions([Query("country")] string country = "GB", CancellationToken cancellationToken = default);
}
