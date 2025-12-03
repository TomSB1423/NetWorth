using FluentValidation;
using Networth.Application.Commands;

namespace Networth.Application.Validators;

/// <summary>
///     Validator for SyncAccountCommand.
/// </summary>
public class SyncAccountCommandValidator : AbstractValidator<SyncAccountCommand>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SyncAccountCommandValidator"/> class.
    /// </summary>
    public SyncAccountCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.DateFrom)
            .LessThanOrEqualTo(x => x.DateTo ?? DateTimeOffset.UtcNow)
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue)
            .WithMessage("DateFrom must be before or equal to DateTo");
    }
}
