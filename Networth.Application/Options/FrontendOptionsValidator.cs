using FluentValidation;

namespace Networth.Application.Options;

public class FrontendOptionsValidator : AbstractValidator<FrontendOptions>
{
    public FrontendOptionsValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty()
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Frontend URL must be a valid absolute URI.");
    }
}
