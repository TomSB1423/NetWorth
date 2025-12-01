using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Networth.Infrastructure.Extensions;
using Networth.Infrastructure.Gocardless.Options;

namespace Networth.Functions.Tests.Integration.Infrastructure;

public class OptionsValidationTests
{
    [Fact]
    public void GocardlessOptions_WithValidConfiguration_ShouldValidateSuccessfully()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Gocardless:BankAccountDataBaseUrl"] = "https://bankaccountdata.gocardless.com",
                ["Gocardless:SecretId"] = "test-secret-id",
                ["Gocardless:SecretKey"] = "test-secret-key",
            })
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        var options = serviceProvider.GetRequiredService<IOptions<GocardlessOptions>>();
        var exception = Record.Exception(() => _ = options.Value);

        Assert.Null(exception);
    }

    [Fact]
    public void GocardlessOptions_WithMissingBaseUrl_ShouldThrowOptionsValidationException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Gocardless:BankAccountDataBaseUrl"] = string.Empty,
                ["Gocardless:SecretId"] = "test-secret-id",
                ["Gocardless:SecretKey"] = "test-secret-key",
            })
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        var options = serviceProvider.GetRequiredService<IOptions<GocardlessOptions>>();
        var exception = Assert.Throws<OptionsValidationException>(() => _ = options.Value);

        Assert.Contains("Bank Account Data Base URL is required", exception.Message);
    }
}
