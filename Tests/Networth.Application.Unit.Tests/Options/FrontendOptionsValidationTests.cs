using FluentValidation.Results;
using Networth.Application.Options;

namespace Networth.Application.Unit.Tests.Options;

public class FrontendOptionsValidationTests
{
    [Fact]
    public void FrontendOptions_WithValidUrl_ShouldValidateSuccessfully()
    {
        // Arrange
        FrontendOptions options = new()
        {
            Url = "https://networth.app",
        };
        FrontendOptionsValidator validator = new();

        // Act
        ValidationResult? result = validator.Validate(options);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("invalid-url")]
    [InlineData("/relative/path")]
    public void FrontendOptions_WithInvalidUrl_ShouldFailValidation(string? url)
    {
        // Arrange
        FrontendOptions options = new()
        {
            Url = url!,
        };
        FrontendOptionsValidator validator = new();

        // Act
        ValidationResult? result = validator.Validate(options);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(FrontendOptions.Url));
    }
}
