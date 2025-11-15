using System.Net;
using Microsoft.Extensions.Logging;

namespace Networth.Infrastructure.Extensions;

/// <inheritdoc />
public class RefitRetryHandler(ILogger<RefitRetryHandler> logger) : DelegatingHandler
{
    private const int MaxRetries = 5;
    private const int BaseDelayMilliseconds = 1000;

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage? response = null;

        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                logger.LogDebug(
                    "Request to {RequestUri} succeeded on attempt {Attempt}",
                    request.RequestUri,
                    attempt + 1);
                return response;
            }

            // Check if we should retry
            if (attempt < MaxRetries - 1 && ShouldRetry(response.StatusCode))
            {
                var delay = CalculateDelay(attempt, response);

                logger.LogWarning(
                    "Request to {RequestUri} failed with status {StatusCode}. Retrying in {Delay}ms (attempt {Attempt}/{MaxRetries})",
                    request.RequestUri,
                    response.StatusCode,
                    delay.TotalMilliseconds,
                    attempt + 1,
                    MaxRetries);

                await Task.Delay(delay, cancellationToken);
            }
            else
            {
                logger.LogError(
                    "Request to {RequestUri} failed with status {StatusCode} after {Attempts} attempts",
                    request.RequestUri,
                    response.StatusCode,
                    attempt + 1);
                break;
            }
        }

        return response!;
    }

    private static bool ShouldRetry(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.TooManyRequests => true,
            HttpStatusCode.ServiceUnavailable => true,
            HttpStatusCode.GatewayTimeout => true,
            HttpStatusCode.RequestTimeout => true,
            _ when (int)statusCode >= 500 => true,
            _ => false
        };
    }

    private static TimeSpan CalculateDelay(int attempt, HttpResponseMessage response)
    {
        // Check for Retry-After header
        if (response.Headers.RetryAfter?.Delta.HasValue == true)
        {
            return response.Headers.RetryAfter.Delta.Value;
        }

        if (response.Headers.RetryAfter?.Date.HasValue == true)
        {
            var retryAfter = response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
            if (retryAfter > TimeSpan.Zero)
            {
                return retryAfter;
            }
        }

        // Exponential backoff: 1s, 2s, 4s, 8s, 16s
        var exponentialDelay = BaseDelayMilliseconds * Math.Pow(2, attempt);

        // Add jitter (random 0-1000ms) to prevent thundering herd
        var jitter = Random.Shared.Next(0, 1000);

        return TimeSpan.FromMilliseconds(exponentialDelay + jitter);
    }
}
