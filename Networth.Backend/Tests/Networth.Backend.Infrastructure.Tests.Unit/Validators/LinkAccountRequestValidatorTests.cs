using FluentValidation.Results;
using Networth.Backend.Functions.Models.Requests;
using Networth.Backend.Functions.Validators;

namespace Networth.Backend.Infrastructure.Tests.Unit.Validators;

/// <summary>
///     Unit tests for LinkAccountRequestValidator.
/// </summary>
public class LinkAccountRequestValidatorTests
{
    private readonly LinkAccountRequestValidator _validator;

    public LinkAccountRequestValidatorTests()
    {
        _validator = new LinkAccountRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_InstitutionId_Is_Empty()
    {
        // Arrange
        LinkAccountRequest request = new()
        {
            InstitutionId = string.Empty,
            RedirectUrl = "https://example.com",
            Reference = "test-ref",
        };

        // Act
        ValidationResult result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(request.InstitutionId));
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Institution ID is required");
    }

    [Fact]
    public void Should_Have_Error_When_RedirectUrl_Is_Invalid()
    {
        // Arrange
        LinkAccountRequest request = new()
        {
            InstitutionId = "test-institution",
            RedirectUrl = "not-a-valid-url",
            Reference = "test-ref",
        };

        // Act
        ValidationResult result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(request.RedirectUrl));
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Redirect URL must be a valid URL");
    }

    [Fact]
    public void Should_Have_Error_When_Reference_Is_Too_Long()
    {
        // Arrange
        string longReference = new('a', 101); // 101 characters
        LinkAccountRequest request = new()
        {
            InstitutionId = "test-institution",
            RedirectUrl = "https://example.com",
            Reference = longReference,
        };

        // Act
        ValidationResult result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(request.Reference));
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Reference must not exceed 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_MaxHistoricalDays_Is_Zero()
    {
        // Arrange
        LinkAccountRequest request = new()
        {
            InstitutionId = "test-institution",
            RedirectUrl = "https://example.com",
            Reference = "test-ref",
            MaxHistoricalDays = 0,
        };

        // Act
        ValidationResult result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(request.MaxHistoricalDays));
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Max historical days must be greater than 0");
    }

    [Fact]
    public void Should_Have_Error_When_UserLanguage_Is_Invalid_Format()
    {
        // Arrange
        LinkAccountRequest request = new()
        {
            InstitutionId = "test-institution",
            RedirectUrl = "https://example.com",
            Reference = "test-ref",
            UserLanguage = "en", // Should be uppercase
        };

        // Act
        ValidationResult result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(request.UserLanguage));
        Assert.Contains(result.Errors, e => e.ErrorMessage == "User language must be in uppercase format (e.g., 'EN', 'DE')");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        LinkAccountRequest request = new()
        {
            InstitutionId = "test-institution",
            RedirectUrl = "https://example.com/callback",
            Reference = "test-ref",
            MaxHistoricalDays = 90,
            AccessValidForDays = 90,
            UserLanguage = "EN",
        };

        // Act
        ValidationResult result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
