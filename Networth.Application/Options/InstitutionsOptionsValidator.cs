using FluentValidation;

namespace Networth.Application.Options;

/// <summary>
///     Validator for Institutions options configuration.
/// </summary>
public class InstitutionsOptionsValidator : AbstractValidator<InstitutionsOptions>
{
    public InstitutionsOptionsValidator()
    {
        // UseSandbox is a boolean with no specific validation requirements
        // It defaults to false and both true/false are valid values
    }
}
