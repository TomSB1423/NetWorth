using FluentValidation;
using Networth.Application.Queries;

namespace Networth.Application.Validators;

/// <summary>
///     Validator for GetTransactionsQuery.
/// </summary>
public class GetTransactionsQueryValidator : AbstractValidator<GetTransactionsQuery>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GetTransactionsQueryValidator"/> class.
    /// </summary>
    public GetTransactionsQueryValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID is required");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be at least 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }
}
