using Microsoft.Extensions.Logging;

namespace Networth.Backend.Infrastructure.Extensions;

/// <inheritdoc />
public class RefitLoggingHandler(ILogger<RefitLoggingHandler> logger) : DelegatingHandler
{
    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Status Code: {Status Code} Body: {@Body}", request.RequestUri, request.Content);
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Status Code: {Response} Body: {@Body}", response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
        return response;
    }
}
