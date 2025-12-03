using FluentValidation;
using Networth.Application.Commands;

namespace Networth.Application.Validators;

/// <summary>
///     Validator for SyncInstitutionCommand.
/// </summary>
public class SyncInstitutionCommandValidator : AbstractValidator<SyncInstitutionCommand>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SyncInstitutionCommandValidator"/> class.
    /// </summary>
    public SyncInstitutionCommandValidator()
    {
        RuleFor(x => x.InstitutionId)
            .NotEmpty()
            .WithMessage("Institution ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.DateFrom)
            .LessThanOrEqualTo(x => x.DateTo ?? DateTimeOffset.UtcNow)
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue)
            .WithMessage("DateFrom must be before or equal to DateTo");
    }
}
