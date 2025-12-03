using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Networth.Infrastructure.Extensions;

/// <summary>
///     Extension methods for validating options using FluentValidation.
/// </summary>
public static class OptionsBuilderFluentValidationExtensions
{
    /// <summary>
    ///     Validates options using FluentValidation by retrieving the validator from the service provider.
    /// </summary>
    /// <typeparam name="TOptions">The options type to validate.</typeparam>
    /// <param name="optionsBuilder">The options builder.</param>
    /// <returns>The options builder for chaining.</returns>
    public static OptionsBuilder<TOptions> ValidateFluently<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder)
        where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
            serviceProvider => new FluentValidationOptions<TOptions>(
                optionsBuilder.Name,
                serviceProvider.GetRequiredService<IValidator<TOptions>>()));

        return optionsBuilder;
    }
}
