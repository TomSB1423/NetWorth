using FluentValidation.Results;
using Networth.Infrastructure.Gocardless.Options;

namespace Networth.Infrastructure.Unit.Tests;

/// <summary>
///     Tests for validating configuration options.
/// </summary>
public class OptionsValidationTests
{
    [Fact]
    public void GocardlessOptions_WithValidConfiguration_ShouldValidateSuccessfully()
    {
        // Arrange
        GocardlessOptions options = new()
        {
            BankAccountDataBaseUrl = "https://bankaccountdata.gocardless.com", SecretId = "test-secret-id", SecretKey = "test-secret-key",
        };
        GocardlessOptionsValidator validator = new();

        // Act
        ValidationResult? result = validator.Validate(options);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void GocardlessOptions_WithMissingBaseUrl_ShouldFailValidation()
    {
        // Arrange
        GocardlessOptions options = new()
        {
            BankAccountDataBaseUrl = string.Empty, SecretId = "test-secret-id", SecretKey = "test-secret-key",
        };
        GocardlessOptionsValidator validator = new();

        // Act
        ValidationResult? result = validator.Validate(options);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Bank Account Data Base URL is required");
    }
}
