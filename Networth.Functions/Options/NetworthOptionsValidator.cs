using FluentValidation;

namespace Networth.Functions.Options;

/// <summary>
///     Validator for Networth options configuration.
/// </summary>
public class NetworthOptionsValidator : AbstractValidator<NetworthOptions>
{
    public NetworthOptionsValidator()
    {
        // MockAuthentication is a boolean with no specific validation requirements
        // It defaults to false and both true/false are valid values
    }
}
