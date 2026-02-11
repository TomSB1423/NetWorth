using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Networth.Functions.Tests.Integration.Infrastructure;
using Networth.Infrastructure.Data.Context;
using Networth.Infrastructure.Gocardless.Options;
using Networth.ServiceDefaults;
using Projects;
using Xunit;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration.Authentication;

/// <summary>
///     Integration tests for custom mock user configuration.
///     Verifies that mock user settings can be customized via environment variables.
/// </summary>
[Collection("Integration")]
public class CustomMockUserConfigurationTests : IAsyncLifetime
{
    private const string CustomFirebaseUid = "custom-test-uid-456";
    private const string CustomName = "Custom Test User";
    private const string CustomEmail = "custom@test.com";

    private readonly ITestOutputHelper _testOutput;
    private DistributedApplication _app = null!;
    private HttpClient _httpClient = null!;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CustomMockUserConfigurationTests"/> class.
    /// </summary>
    public CustomMockUserConfigurationTests(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        _app = await CreateAppWithCustomMockUserAsync();
        _httpClient = _app.CreateHttpClient(ResourceNames.Functions);

        // Ensure database schema is created
        await EnsureDatabaseMigratedAsync();
    }

    /// <inheritdoc />
    public async Task DisposeAsync() => await _app.DisposeAsync();

    [Fact]
    public async Task CreateUser_WithCustomMockUserConfiguration_UsesCustomDetails()
    {
        // Arrange & Act
        var response = await _httpClient.PostAsync("/api/users", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(content);

        // Verify the custom mock user details are used (firebaseUid is not included in the response)
        Assert.Equal(CustomName, jsonDoc.RootElement.GetProperty("name").GetString());
        Assert.Equal(CustomEmail, jsonDoc.RootElement.GetProperty("email").GetString());
    }

    [Fact]
    public async Task GetCurrentUser_WithCustomMockUserConfiguration_ReturnsCustomDetails()
    {
        // Arrange - Create the user first
        await _httpClient.PostAsync("/api/users", null);

        // Act
        var response = await _httpClient.GetAsync("/api/users/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(content);

        // Verify the custom mock user details are returned (firebaseUid is not included in the response)
        Assert.Equal(CustomName, jsonDoc.RootElement.GetProperty("name").GetString());
        Assert.Equal(CustomEmail, jsonDoc.RootElement.GetProperty("email").GetString());
    }

    /// <summary>
    ///     Creates a distributed application with custom mock user configuration.
    /// </summary>
    private async Task<DistributedApplication> CreateAppWithCustomMockUserAsync()
    {
        IDistributedApplicationTestingBuilder builder = await DistributedApplicationTestingBuilder.CreateAsync<Networth_AppHost>(
            [],
            (appOptions, hostSettings) =>
            {
                hostSettings.EnvironmentName = "Development";
                appOptions.DisableDashboard = true;
            });

        // Provide default values for all required parameters
        builder.Configuration["Parameters:firebase-api-key"] = "test-api-key";
        builder.Configuration["Parameters:firebase-auth-domain"] = "test.firebaseapp.com";
        builder.Configuration["Parameters:firebase-project-id"] = "test-project";
        builder.Configuration["Parameters:mock-authentication"] = "true";

        builder.WithRandomVolumeNames();

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddXUnit(_testOutput);
            logging.SetMinimumLevel(LogLevel.Information);
            logging.AddFilter("Aspire", LogLevel.Warning);
        });

        builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        ProjectResource functionsResource = builder.Resources
            .OfType<ProjectResource>()
            .First(r => r.Name == ResourceNames.Functions);

        // Configure custom mock user via environment variables
        functionsResource.Annotations.Add(new EnvironmentCallbackAnnotation(env =>
        {
            env.EnvironmentVariables["Networth__UseAuthentication"] = "false";
            env.EnvironmentVariables["Networth__MockUser__FirebaseUid"] = CustomFirebaseUid;
            env.EnvironmentVariables["Networth__MockUser__Name"] = CustomName;
            env.EnvironmentVariables["Networth__MockUser__Email"] = CustomEmail;
            env.EnvironmentVariables["Firebase__ProjectId"] = "disabled";

            // Configure Gocardless for basic operation
            env.EnvironmentVariables[$"Gocardless:{nameof(GocardlessOptions.BankAccountDataBaseUrl)}"] = "https://mock.invalid/api/v2";
            env.EnvironmentVariables[$"Gocardless:{nameof(GocardlessOptions.SecretId)}"] = "test-secret-id";
            env.EnvironmentVariables[$"Gocardless:{nameof(GocardlessOptions.SecretKey)}"] = "test-secret-key";
        }));

        // Disable queue triggers
        functionsResource.Annotations.Add(new EnvironmentCallbackAnnotation(env =>
        {
            env.EnvironmentVariables["AzureWebJobs.CalculateRunningBalance.Disabled"] = "true";
            env.EnvironmentVariables["AzureWebJobs.SyncInstitutionQueue.Disabled"] = "true";
            env.EnvironmentVariables["AzureWebJobs.SyncAccount.Disabled"] = "true";
        }));

        DistributedApplication app = await builder.BuildAsync();
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(180));
        await app.StartAsync(cts.Token);
        await app.ResourceNotifications.WaitForResourceHealthyAsync(
            ResourceNames.Functions,
            cts.Token);

        return app;
    }

    private async Task EnsureDatabaseMigratedAsync()
    {
        string? dbConnectionString = await _app.GetConnectionStringAsync(ResourceNames.NetworthDb);

        ServiceCollection services = new();
        services.AddDbContext<NetworthDbContext>(options =>
        {
            options.UseNpgsql(dbConnectionString);
            options.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        NetworthDbContext dbContext = scope.ServiceProvider.GetRequiredService<NetworthDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}
