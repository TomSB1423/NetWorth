using FluentValidation;
using Microsoft.Extensions.Options;

namespace Networth.Infrastructure.Extensions;

/// <summary>
///     Validates options using FluentValidation.
/// </summary>
/// <typeparam name="TOptions">The options type to validate.</typeparam>
internal class FluentValidationOptions<TOptions> : IValidateOptions<TOptions>
    where TOptions : class
{
    private readonly IValidator<TOptions> _validator;
    private readonly string _name;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FluentValidationOptions{TOptions}"/> class.
    /// </summary>
    /// <param name="name">The name of the options instance.</param>
    /// <param name="validator">The FluentValidation validator.</param>
    public FluentValidationOptions(string name, IValidator<TOptions> validator)
    {
        _name = name;
        _validator = validator;
    }

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? optionsName, TOptions options)
    {
        if (optionsName != null && optionsName != _name)
        {
            return ValidateOptionsResult.Skip;
        }

        ArgumentNullException.ThrowIfNull(options);

        var result = _validator.Validate(options);

        if (result.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        var errors = result.Errors.Select(e =>
            $"Options validation failed for '{e.PropertyName}': {e.ErrorMessage}").ToList();
        return ValidateOptionsResult.Fail(errors);
    }
}
