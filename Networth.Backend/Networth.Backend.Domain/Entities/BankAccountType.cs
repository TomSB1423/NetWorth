namespace Networth.Backend.Domain.Entities;

/// <summary>
///     Represents the different types of bank accounts.
/// </summary>
public enum BankAccountType
{
    /// <summary>
    ///     A checking account for everyday transactions.
    /// </summary>
    Checking,

    /// <summary>
    ///     A spending account for everyday transactions.
    /// </summary>
    Spending,

    /// <summary>
    ///     A savings account for storing money and earning interest.
    /// </summary>
    Savings,

    /// <summary>
    ///     A credit account that allows borrowing up to a credit limit.
    /// </summary>
    Credit,

    /// <summary>
    ///     An investment account for trading securities.
    /// </summary>
    Investment,

    /// <summary>
    ///     A loan account representing borrowed money.
    /// </summary>
    Loan,

    /// <summary>
    ///     A mortgage account for home loans.
    /// </summary>
    Mortgage
}
