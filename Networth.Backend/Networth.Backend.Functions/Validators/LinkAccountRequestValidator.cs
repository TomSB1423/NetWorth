using FluentValidation;
using Networth.Backend.Functions.Models.Requests;

namespace Networth.Backend.Functions.Validators;

/// <summary>
///     Validator for LinkAccountRequest.
/// </summary>
public class LinkAccountRequestValidator : AbstractValidator<LinkAccountRequest>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkAccountRequestValidator"/> class.
    /// </summary>
    public LinkAccountRequestValidator()
    {
        RuleFor(x => x.InstitutionId)
            .NotEmpty()
            .WithMessage("Institution ID is required");

        RuleFor(x => x.RedirectUrl)
            .NotEmpty()
            .WithMessage("Redirect URL is required")
            .Must(BeAValidUrl)
            .WithMessage("Redirect URL must be a valid URL");

        RuleFor(x => x.Reference)
            .NotEmpty()
            .WithMessage("Reference is required")
            .MaximumLength(100)
            .WithMessage("Reference must not exceed 100 characters");

        RuleFor(x => x.MaxHistoricalDays)
            .GreaterThan(0)
            .WithMessage("Max historical days must be greater than 0")
            .LessThanOrEqualTo(730)
            .WithMessage("Max historical days cannot exceed 730 days (2 years)");

        RuleFor(x => x.AccessValidForDays)
            .GreaterThan(0)
            .WithMessage("Access valid for days must be greater than 0")
            .LessThanOrEqualTo(180)
            .WithMessage("Access valid for days cannot exceed 180 days");

        RuleFor(x => x.UserLanguage)
            .NotEmpty()
            .WithMessage("User language is required")
            .Length(2)
            .WithMessage("User language must be a 2-character language code")
            .Matches("^[A-Z]{2}$")
            .WithMessage("User language must be in uppercase format (e.g., 'EN', 'DE')");
    }

    private static bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri? result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
