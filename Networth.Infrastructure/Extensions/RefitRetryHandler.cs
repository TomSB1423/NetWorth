using Microsoft.Extensions.Logging;

namespace Networth.Infrastructure.Extensions;

/// <inheritdoc />
public class RefitRetryHandler(ILogger<RefitRetryHandler> logger) : DelegatingHandler
{
    private const int MaxRetries = 5;

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
        int requestNo = 1;
        while (requestNo < MaxRetries)
        {
            if (response.IsSuccessStatusCode)
            {
                break;
            }

            response = await base.SendAsync(request, cancellationToken);
            requestNo++;
        }

        logger.LogDebug("Status Code: {Status Code} Body: {@Body}. No Reqs: {@RequestsNo}", request.RequestUri, request.Content, requestNo);
        return response;
    }
}
