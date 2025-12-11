using System.Net;
using Microsoft.Extensions.Logging;

namespace Networth.Infrastructure.Gocardless.Handlers;

/// <summary>
///     Handler that logs rate limit responses (HTTP 429) from the GoCardless API.
/// </summary>
public class RateLimitLoggingHandler(ILogger<RateLimitLoggingHandler> logger) : DelegatingHandler
{
    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            var retryAfter = response.Headers.RetryAfter;
            logger.LogWarning(
                "GoCardless Rate Limit Hit: {Method} {Uri}. Retry-After: {RetryAfter}s",
                request.Method,
                request.RequestUri,
                retryAfter?.Delta?.TotalSeconds ?? 0);
        }

        return response;
    }
}
