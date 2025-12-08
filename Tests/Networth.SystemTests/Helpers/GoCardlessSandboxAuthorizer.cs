using Microsoft.Playwright;

namespace Networth.SystemTests.Helpers;

/// <summary>
///     Helper class to programmatically authorize GoCardless sandbox requisitions.
///     Uses Playwright to automate the browser-based OAuth flow.
/// </summary>
public class GoCardlessSandboxAuthorizer : IAsyncDisposable
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private bool _disposed;

    /// <summary>
    ///     Authorizes a GoCardless sandbox requisition by automating the OAuth flow.
    /// </summary>
    /// <param name="authorizationLink">The authorization link from the LinkAccount response.</param>
    /// <param name="headless">Whether to run browser in headless mode (default: true).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task AuthorizeRequisitionAsync(
        string authorizationLink,
        bool headless = true,
        CancellationToken cancellationToken = default)
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = headless,
        });

        var context = await _browser.NewContextAsync();
        var page = await context.NewPageAsync();

        try
        {
            // Step 1: Navigate to GoCardless consent page
            await NavigateWithRetryAsync(page, authorizationLink, cancellationToken);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Click "Agree and continue" on the GoCardless consent page
            var agreeButton = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Agree and continue" });
            await agreeButton.ClickAsync();

            // Step 2: Sandbox Finance login page (fields should be pre-filled)
            // Wait for the Sign in button to appear
            var signInButton = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Sign in" });

            // Click Sign in
            await signInButton.ClickAsync();

            // Step 3: Sandbox Finance approval/consent page
            // Wait for the Approve button/link to appear
            var approveLink = page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Approve" });

            // Click Approve
            await approveLink.ClickAsync();

            // Wait a moment for any final processing
            await page.WaitForTimeoutAsync(1000);
        }
        finally
        {
            await page.CloseAsync();
            await context.CloseAsync();
        }
    }

    /// <summary>
    ///     Disposes of Playwright resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        if (_browser != null)
        {
            await _browser.CloseAsync();
            await _browser.DisposeAsync();
        }

        _playwright?.Dispose();
        _disposed = true;

        GC.SuppressFinalize(this);
    }

    private async Task NavigateWithRetryAsync(IPage page, string url, CancellationToken cancellationToken)
    {
        const int maxRetries = 3;
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await page.GotoAsync(url);
                return;
            }
            catch (PlaywrightException ex) when (ex.Message.Contains("ERR_NETWORK_CHANGED") || ex.Message.Contains("ERR_CONNECTION_RESET"))
            {
                if (i == maxRetries - 1)
                {
                    throw;
                }

                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}
