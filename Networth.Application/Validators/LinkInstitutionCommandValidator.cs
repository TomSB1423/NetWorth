using FluentValidation;
using Networth.Application.Commands;

namespace Networth.Application.Validators;

/// <summary>
///     Validator for LinkInstitutionCommand.
/// </summary>
public class LinkInstitutionCommandValidator : AbstractValidator<LinkInstitutionCommand>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkInstitutionCommandValidator"/> class.
    /// </summary>
    public LinkInstitutionCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.InstitutionId)
            .NotEmpty()
            .WithMessage("Institution ID is required");
    }
}
