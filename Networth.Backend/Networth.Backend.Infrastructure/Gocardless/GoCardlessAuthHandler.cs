using System.Net.Http.Headers;

namespace Networth.Backend.Infrastructure.Gocardless;

internal class GoCardlessAuthHandler(GoCardlessTokenManager tokenManager)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await tokenManager.GetValidTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await base.SendAsync(request, cancellationToken);

        return response;
    }
}
