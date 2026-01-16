using FluentValidation;

namespace Networth.Functions.Options;

/// <summary>
///     Validator for Networth options configuration.
/// </summary>
public class NetworthOptionsValidator : AbstractValidator<NetworthOptions>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NetworthOptionsValidator" /> class.
    /// </summary>
    public NetworthOptionsValidator()
    {
        // UseAuthentication is a boolean with no specific validation requirements
        // When false, MockUser configuration is used
        RuleFor(x => x.MockUser).NotNull();
        RuleFor(x => x.MockUser.FirebaseUid).NotEmpty().When(x => !x.UseAuthentication);
        RuleFor(x => x.MockUser.Name).NotEmpty().When(x => !x.UseAuthentication);
        RuleFor(x => x.MockUser.Email).NotEmpty().EmailAddress().When(x => !x.UseAuthentication);
    }
}
