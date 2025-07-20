using System.Net.Http.Headers;

namespace Networth.Backend.Infrastructure.Gocardless;

internal class GoCardlessAuthHandler : DelegatingHandler
{
    private readonly GoCardlessTokenManager _tokenManager;

    public GoCardlessAuthHandler(GoCardlessTokenManager tokenManager)
    {
        _tokenManager = tokenManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenManager.GetValidTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await base.SendAsync(request, cancellationToken);

        return response;
    }
}
