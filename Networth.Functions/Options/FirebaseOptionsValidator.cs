using FluentValidation;

namespace Networth.Functions.Options;

public class FirebaseOptionsValidator : AbstractValidator<FirebaseOptions>
{
    public FirebaseOptionsValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Firebase Project ID is required");
    }
}
