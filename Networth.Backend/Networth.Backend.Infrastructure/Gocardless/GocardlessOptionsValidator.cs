using FluentValidation;

namespace Networth.Backend.Infrastructure.Gocardless;

/// <summary>
/// Validator for GoCardless options configuration.
/// </summary>
public class GocardlessOptionsValidator : AbstractValidator<GocardlessOptions>
{
    public GocardlessOptionsValidator()
    {
        RuleFor(x => x.BankAccountDataBaseUrl)
            .NotEmpty()
            .WithMessage("Bank Account Data Base URL is required")
            .Must(BeValidUrl)
            .WithMessage("Bank Account Data Base URL must be a valid URL");

        RuleFor(x => x.SecretId)
            .NotEmpty()
            .WithMessage("Secret ID is required")
            .MinimumLength(1)
            .WithMessage("Secret ID cannot be empty");

        RuleFor(x => x.SecretKey)
            .NotEmpty()
            .WithMessage("Secret Key is required")
            .MinimumLength(1)
            .WithMessage("Secret Key cannot be empty");
    }

    /// <summary>
    /// Validates if the provided string is a valid URL.
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <returns>True if the URL is valid, otherwise false.</returns>
    private static bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
