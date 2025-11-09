using FluentValidation;

namespace Networth.Infrastructure.Data.Options;

/// <summary>
///     Validator for <see cref="DatabaseOptions" /> to ensure valid configuration.
/// </summary>
public class DataBaseOptionsValidator : AbstractValidator<DatabaseOptions>
{
    /// <inheritdoc />
    public DataBaseOptionsValidator() =>
        RuleFor(options => options.ConnectionString)
            .NotEmpty()
            .WithMessage("Database connection string must not be empty.");
}
