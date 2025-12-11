using FluentValidation;

namespace Networth.Application.Options;

public class FrontendOptionsValidator : AbstractValidator<FrontendOptions>
{
    public FrontendOptionsValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty()
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out var outUri) &&
                         (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps))
            .WithMessage("Frontend URL must be a valid HTTP or HTTPS URI.");
    }
}
