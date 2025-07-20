using Networth.Backend.Domain.Entities;
using Refit;

namespace Networth.Backend.Infrastructure.Gocardless;

internal interface IGocardlessClient
{
    [Post("/token/new")]
    Task<TokenResponse> GetAccessTokenAsync([Body] TokenRequest request);

    [Get("/institutions")]
    Task<IEnumerable<Institution>> GetInstitutions([Query] string country, CancellationToken cancellationToken = default);
}
