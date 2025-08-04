using Microsoft.Extensions.Logging;

namespace Networth.Backend.Infrastructure.Extensions;

/// <inheritdoc />
public class RefitLoggingHandler(ILogger<RefitLoggingHandler> logger) : DelegatingHandler
{
    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Request: {Request}", request);
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Response: {Response}", response);
        return response;
    }
}
