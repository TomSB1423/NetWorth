using System.Text.Json;
using Aspire.Hosting;
using Microsoft.Extensions.Logging;
using Networth.Backend.Functions.Tests.Integration.Fixtures;
using Networth.Backend.Functions.Tests.Integration.Infrastructure;
using Xunit.Abstractions;

namespace Networth.Backend.Functions.Tests.Integration;

/// <summary>
///     Integration tests for the Azure Functions backend using shared Mockoon fixture.
/// </summary>
public class FunctionsIntegrationTests : IClassFixture<MockoonTestFixture>
{
    private readonly MockoonTestFixture _mockoonFixture;
    private readonly ITestOutputHelper _output;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FunctionsIntegrationTests"/> class.
    /// </summary>
    /// <param name="mockoonFixture">The shared Mockoon test fixture.</param>
    /// <param name="output">The test output helper for logging.</param>
    public FunctionsIntegrationTests(MockoonTestFixture mockoonFixture, ITestOutputHelper output)
    {
        _mockoonFixture = mockoonFixture;
        _output = output;
    }

    /// <summary>
    ///     Tests that the GetInstitutions endpoint returns OK status code and valid JSON response.
    /// </summary>
    [Fact]
    public async Task GetInstitutionsEndpointReturnsOkStatusCodeAndValidJson()
    {
        // Arrange
        await using var app = await CreateTestBuilderAsync(_mockoonFixture.MockoonBaseUrl);
        await app.StartAsync();
        await app.StartAsync();

        // Act
        using var cts = new CancellationTokenSource(TestTimeouts.Default);


        // Act
        _output.WriteLine($"Mockoon base URL: {_mockoonFixture.MockoonBaseUrl}");

        // Log initial resource states
        _output.WriteLine("=== Starting Aspire Integration Test ===");

        _output.WriteLine($"Test timeout: {TestTimeouts.Default.TotalMinutes} minutes");
        var httpClient = app.CreateHttpClient(AspireResourceNames.Functions);
        await LogResourceStatesAsync(app, _output);

        // Create HTTP client and wait for Functions to be ready by polling the endpoint
        var httpClient = app.CreateHttpClient(AspireResourceNames.Functions);
        httpClient.Timeout = TimeSpan.FromSeconds(10); // Set per-request timeout

        _output.WriteLine($"HTTP client created for '{AspireResourceNames.Functions}'");

        // Poll the endpoint until it responds (Functions app might still be migrating database)
        // Use 3 minutes of polling to leave buffer before the 5-minute test timeout
        var maxAttempts = 36; // 36 attempts with 5 second delays = 180 seconds (3 minutes) max
        var delayBetweenAttempts = TimeSpan.FromSeconds(5);
        HttpResponseMessage? response = null;
        var lastException = default(Exception);

        _output.WriteLine($"Starting polling loop: {maxAttempts} attempts with {delayBetweenAttempts.TotalSeconds}s delays");

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cts.Token.ThrowIfCancellationRequested();

            try
            {
                using var requestCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
                requestCts.CancelAfter(TimeSpan.FromSeconds(10));

                _output.WriteLine($"[Attempt {attempt}/{maxAttempts}] Sending GET request to /api/institutions...");

                response = await httpClient.GetAsync("/api/institutions", requestCts.Token);

                _output.WriteLine($"[Attempt {attempt}/{maxAttempts}] Got response: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    _output.WriteLine($"[Attempt {attempt}/{maxAttempts}] SUCCESS! Endpoint is ready.");
                    break;
                }

                lastException = new InvalidOperationException($"Attempt {attempt}: Got status code {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                // Functions app not ready yet, continue polling
                _output.WriteLine($"[Attempt {attempt}/{maxAttempts}] HttpRequestException: {ex.Message}");
                lastException = ex;
            }
            catch (TaskCanceledException ex) when (!cts.Token.IsCancellationRequested)
            {
                // Request timeout (not overall timeout), continue polling
                _output.WriteLine($"[Attempt {attempt}/{maxAttempts}] Request timeout (10s)");
                lastException = ex;
            }
            catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
            {
                // Overall test timeout reached
                _output.WriteLine($"[Attempt {attempt}/{maxAttempts}] OVERALL TEST TIMEOUT REACHED!");
                await LogResourceStatesAsync(app, _output);
                throw new TimeoutException($"Test timeout reached after {attempt} attempts. Functions endpoint never became ready.", lastException);
            }

            if (attempt < maxAttempts)
            {
                // Log resource states every 5 attempts
                if (attempt % 5 == 0)
                {
                    await LogResourceStatesAsync(app, _output);
                }

                try
                {
                    _output.WriteLine($"[Attempt {attempt}/{maxAttempts}] Waiting {delayBetweenAttempts.TotalSeconds}s before next attempt...");
                    await Task.Delay(delayBetweenAttempts, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // Overall test timeout reached during delay
                    _output.WriteLine($"[Attempt {attempt}/{maxAttempts}] TIMEOUT during delay!");
                    await LogResourceStatesAsync(app, _output);
                    throw new TimeoutException($"Test timeout reached after {attempt} attempts while waiting between retries. Functions endpoint never became ready.", lastException);
                }
            }
            _output.WriteLine("=== MAX ATTEMPTS REACHED ===");
            await LogResourceStatesAsync(app, _output);

        }
            var lastExMsg = lastException != null ? $"Last exception: {lastException.Message}" : "No exceptions";


                $"Functions endpoint did not become ready after {maxAttempts} attempts. {statusInfo}. {lastExMsg}",
        {
            _output.WriteLine("=== MAX ATTEMPTS REACHED ===");
        }
        _output.WriteLine("=== Test Successful - Endpoint Ready ===");

        return await appBuilder.BuildAsync();
    }

    /// <summary>
    ///     Logs the current state of all resources in the Aspire app.
    /// </summary>
    private static async Task LogResourceStatesAsync(DistributedApplication app, ITestOutputHelper output)
    {
        output.WriteLine("--- Resource States ---");

        try
        {
            var resourceNames = new[] { "postgres", "networth-db", "functions", "funcstoragecf0b0", "react" };

            foreach (var resourceName in resourceNames)
            {
                try
                {
                    var resource = app.Resources.FirstOrDefault(r => r.Name == resourceName);
                    if (resource != null)
                    {

        _output.WriteLine("=== All Assertions Passed ===");
                        // Try to get the latest snapshot
                        var snapshot = await app.ResourceNotifications.WaitForResourceAsync(resourceName,
                            r => r.State != null,
                            CancellationToken.None)
                            .WaitAsync(TimeSpan.FromSeconds(1));

                        output.WriteLine($"  {resourceName}: State={snapshot?.State?.Text ?? "unknown"}, Health={snapshot?.HealthStatus ?? "unknown"}");
                    }
                    else
                    {
                        output.WriteLine($"  {resourceName}: NOT FOUND");
                    }
                }
                catch (TimeoutException)
                {
                    output.WriteLine($"  {resourceName}: TIMEOUT getting state");
                }
                catch (Exception ex)
                {
                    output.WriteLine($"  {resourceName}: ERROR - {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            output.WriteLine($"ERROR logging resource states: {ex.Message}");
        }

        output.WriteLine("--- End Resource States ---");
    }

    /// <summary>
    ///     Logs the current state of all resources in the Aspire app.
    /// </summary>
    private static async Task LogResourceStatesAsync(DistributedApplication app, ITestOutputHelper output)
    {
        output.WriteLine("--- Resource States ---");

        try
        {
            var resourceNames = new[] { "postgres", "networth-db", "functions", "funcstoragecf0b0" };

            foreach (var resourceName in resourceNames)
            {
                try
                {
                    // Try to wait for the resource with a short timeout to see if it responds
                    await app.ResourceNotifications.WaitForResourceAsync(
                        resourceName,
                        CancellationToken.None)
                        .WaitAsync(TimeSpan.FromMilliseconds(500));

                    output.WriteLine($"  {resourceName}: Found and responsive");
                }
                catch (TimeoutException)
                {
                    output.WriteLine($"  {resourceName}: Timeout - may not be ready");
                }
                catch (Exception ex)
                {
                    output.WriteLine($"  {resourceName}: ERROR - {ex.GetType().Name}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            output.WriteLine($"ERROR logging resource states: {ex.Message}");
        }

        output.WriteLine("--- End Resource States ---");
    }
}
