using FluentValidation;
using Networth.Backend.Application.Commands;

namespace Networth.Backend.Application.Validators;

/// <summary>
///     Validator for LinkAccountCommand.
/// </summary>
public class LinkAccountCommandValidator : AbstractValidator<LinkAccountCommand>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkAccountCommandValidator"/> class.
    /// </summary>
    public LinkAccountCommandValidator()
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
