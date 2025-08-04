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
    }

    private static bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri? result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
