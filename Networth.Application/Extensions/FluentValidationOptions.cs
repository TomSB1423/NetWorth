using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Networth.Application.Extensions;

/// <summary>
///     Validates options using FluentValidation.
/// </summary>
/// <typeparam name="TOptions">The options type to validate.</typeparam>
internal class FluentValidationOptions<TOptions> : IValidateOptions<TOptions>
    where TOptions : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _name;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FluentValidationOptions{TOptions}"/> class.
    /// </summary>
    /// <param name="name">The name of the options instance.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public FluentValidationOptions(string name, IServiceProvider serviceProvider)
    {
        _name = name;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? optionsName, TOptions options)
    {
        if (optionsName != null && optionsName != _name)
        {
            return ValidateOptionsResult.Skip;
        }

        ArgumentNullException.ThrowIfNull(options);

        using var scope = _serviceProvider.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<IValidator<TOptions>>();

        var result = validator.Validate(options);

        if (result.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        var errors = result.Errors.Select(e =>
            $"Options validation failed for '{e.PropertyName}': {e.ErrorMessage}").ToList();
        return ValidateOptionsResult.Fail(errors);
    }
}
