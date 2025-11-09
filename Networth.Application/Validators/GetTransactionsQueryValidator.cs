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

        RuleFor(x => x.DateFrom)
            .NotEmpty()
            .WithMessage("Start date is required");

        RuleFor(x => x.DateTo)
            .NotEmpty()
            .WithMessage("End date is required");

        RuleFor(x => x)
            .Must(query => query.DateFrom <= query.DateTo)
            .WithMessage("Start date cannot be greater than end date")
            .When(x => x.DateFrom != default && x.DateTo != default);
    }
}
