using System.Globalization;
using Microsoft.Extensions.Logging;
using Networth.Backend.Application.Interfaces;
using Networth.Backend.Domain.Entities;
using Networth.Backend.Domain.Enums;
using Networth.Backend.Infrastructure.Gocardless.DTOs;
using Refit;

namespace Networth.Backend.Infrastructure.Gocardless;

/// <summary>
///     GoCardless implementation of financial provider services.
/// </summary>
internal class GocardlessService(ILogger<GocardlessService> logger, IGocardlessClient gocardlessClient)
    : IFinancialProvider
{
    /// <inheritdoc />
    public async Task<InstitutionMetadata> GetInstitutionAsync(string institutionId, CancellationToken cancellationToken = default)
    {
        GetInstitutionDto institution = await gocardlessClient.GetInstitution(institutionId, cancellationToken);
        bool transactionParse = int.TryParse(institution.TransactionTotalDays, out int transactionTotalDays);
        bool maxAccessParse = int.TryParse(institution.MaxAccessValidForDays, out int maxAccessValidForDays);
        if (!transactionParse || !maxAccessParse)
        {
            logger.LogWarning(
                "Failed to parse transaction days or access valid days for institution {InstitutionId}. Using default values.",
                institution.Id);
        }

        return new InstitutionMetadata
        {
            Id = institution.Id,
            Name = institution.Name,
            TransactionTotalDays = transactionTotalDays,
            MaxAccessValidForDays = maxAccessValidForDays,
            LogoUrl = institution.Logo,
            Bic = institution.Bic,
            Countries = institution.Countries ?? [],
        };
    }

    /// <inheritdoc />
    public async Task<IEnumerable<InstitutionMetadata>> GetInstitutionsAsync(string country, CancellationToken cancellationToken = default)
    {
        IEnumerable<GetInstitutionDto> response = await gocardlessClient.GetInstitutions(country, cancellationToken);

        var institutions = response.Select(dto =>
        {
            bool transactionParse = int.TryParse(dto.TransactionTotalDays, out int transactionTotalDays);
            bool maxAccessParse = int.TryParse(dto.MaxAccessValidForDays, out int maxAccessValidForDays);
            if (!transactionParse || !maxAccessParse)
            {
                logger.LogWarning("Failed to parse transaction days or access valid days for institution {InstitutionId}. Using default values.", dto.Id);
            }

            return new InstitutionMetadata
            {
                Id = dto.Id,
                Name = dto.Name,
                TransactionTotalDays = transactionTotalDays,
                MaxAccessValidForDays = maxAccessValidForDays,
                LogoUrl = dto.Logo,
                Bic = dto.Bic,
                Countries = dto.Countries ?? [],
            };
        });
        return institutions;
    }

    /// <inheritdoc />
    public async Task<Agreement> CreateAgreementAsync(
        string institutionId,
        int? maxHistoricalDays,
        int? accessValidForDays,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Creating agreement for institution {InstitutionId} with {MaxHistoricalDays} historical days and {AccessValidForDays} access days",
            institutionId,
            maxHistoricalDays,
            accessValidForDays);

        CreateAgreementRequestDto request = new()
        {
            InstitutionId = institutionId,
            MaxHistoricalDays = maxHistoricalDays,
            AccessValidForDays = accessValidForDays,
            AccessScope = [AccessScope.Details, AccessScope.Balances, AccessScope.Transactions],
        };

        try
        {
            CreateAgreementResponseDto response = await gocardlessClient.CreateAgreement(request, cancellationToken);

            logger.LogInformation(
                "Successfully created agreement {AgreementId} for institution {InstitutionId}",
                response.Id,
                response.InstitutionId);

            return new Agreement
            {
                Id = response.Id,
                Created = response.Created,
                InstitutionId = response.InstitutionId,
                MaxHistoricalDays = response.MaxHistoricalDays,
                AccessValidForDays = response.AccessValidForDays,
                AccessScope = response.AccessScope,
                Accepted = response.Accepted,
            };
        }
        catch (ApiException ex)
        {
            logger.LogError(
                ex,
                "Failed to create agreement for institution {InstitutionId}. Status: {StatusCode}, Response: {Content}",
                institutionId,
                ex.StatusCode,
                ex.Content);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Requisition> CreateRequisitionAsync(
        string institutionId,
        string agreementId,
        string redirectUrl,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Creating requisition for institution {InstitutionId} with agreement {AgreementId}",
            institutionId,
            agreementId);

        CreateRequisitionRequestDto request = new()
        {
            Redirect = redirectUrl,
            InstitutionId = institutionId,
            Agreement = agreementId,
        };

        CreateRequisitionResponseDto response = await gocardlessClient.CreateRequisition(request, cancellationToken);

        logger.LogInformation(
            "Successfully created requisition {RequisitionId} with status {Status}",
            response.Id,
            response.Status);

        return new Requisition
        {
            Id = response.Id,
            Created = DateTime.UtcNow, // Set current time since GoCardless might not provide this
            Redirect = response.Redirect,
            Status = AccountLinkStatusMapper(response.Status),
            InstitutionId = response.InstitutionId,
            AgreementId = response.Agreement,
            Reference = response.Reference,
            Accounts = response.Accounts,
            AuthenticationLink = response.Link,
            AccountSelection = response.AccountSelection.ToString(),
        };
    }

    /// <inheritdoc />
    public async Task<Requisition> GetRequisitionAsync(string requisitionId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving requisition {RequisitionId}", requisitionId);

        GetRequisitionResponseDto response = await gocardlessClient.GetRequisition(requisitionId, cancellationToken);

        logger.LogInformation(
            "Successfully retrieved requisition {RequisitionId} with status {Status}",
            response.Id,
            response.Status);

        return new Requisition
        {
            Id = response.Id,
            Created = DateTime.UtcNow, // Set current time since GoCardless might not provide this
            Redirect = response.Redirect,
            Status = AccountLinkStatusMapper(response.Status),
            InstitutionId = response.InstitutionId,
            AgreementId = response.Agreement,
            Reference = response.Reference,
            Accounts = response.Accounts,
            AuthenticationLink = response.Link ?? string.Empty,
            AccountSelection = response.AccountSelection.ToString(),
        };
    }

    /// <inheritdoc />
    public async Task<AccountMetadata> GetAccountAsync(string accountId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving account metadata for account {AccountId}", accountId);

        GetAccountResponseDto response = await gocardlessClient.GetAccount(accountId, cancellationToken);

        logger.LogInformation("Successfully retrieved account metadata for account {AccountId}", accountId);

        return new AccountMetadata
        {
            Id = response.Id,
            Status = response.Status,
            InstitutionId = response.InstitutionId,
            Name = response.Name,
        };
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AccountBalance>> GetAccountBalancesAsync(
        string accountId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving account balances for account {AccountId}", accountId);

        GetAccountBalanceResponseDto response = await gocardlessClient.GetAccountBalances(accountId, cancellationToken);

        logger.LogInformation("Successfully retrieved {BalanceCount} balances for account {AccountId}", response.Balances.Length, accountId);

        return response.Balances.Select(balance => new AccountBalance
        {
            Amount = decimal.Parse(balance.BalanceAmount.Amount, CultureInfo.InvariantCulture),
            Currency = balance.BalanceAmount.Currency,
            BalanceType = balance.BalanceType,
            CreditLimitIncluded = balance.CreditLimitIncluded,
            ReferenceDate = ParseDateTime(balance.ReferenceDate),
        });
    }

    /// <inheritdoc />
    public async Task<AccountDetail> GetAccountDetailsAsync(
        string accountId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving account details for account {AccountId}", accountId);

        GetAccountDetailResponseDto response = await gocardlessClient.GetAccountDetails(accountId, cancellationToken);

        logger.LogInformation("Successfully retrieved account details for account {AccountId}", accountId);

        return new AccountDetail
        {
            Id = response.Account.ResourceId,
            Currency = response.Account.Currency,
            Name = response.Account.Name,
            DisplayName = response.Account.DisplayName,
            Product = response.Account.Product,
            CashAccountType = response.Account.CashAccountType,
            Status = response.Account.Status,
        };
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TransactionMetadata>> GetAccountTransactionsAsync(
        string accountId,
        DateTimeOffset dateFrom,
        DateTimeOffset dateTo,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving account transactions for account {AccountId}", accountId);

        string? dateFromStr = dateFrom.UtcDateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        string? dateToStr = dateTo.UtcDateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        var response =
            await gocardlessClient.GetAccountTransactions(accountId, dateFromStr, dateToStr, cancellationToken);

        List<TransactionMetadata> transactions = [];

        // Add booked transactions
        foreach (TransactionDto transaction in response.Transactions.Booked)
        {
            transactions.Add(MapTransactionDto(transaction, accountId, false));
        }

        // Add pending transactions if available
        if (response.Transactions.Pending != null)
        {
            foreach (TransactionDto transaction in response.Transactions.Pending)
            {
                transactions.Add(MapTransactionDto(transaction, accountId, true));
            }
        }

        logger.LogInformation("Successfully retrieved {TransactionCount} transactions for account {AccountId}", transactions.Count, accountId);

        return transactions;
    }

    private static TransactionMetadata MapTransactionDto(TransactionDto dto, string accountId, bool isPending) =>
        new()
        {
            Id = $"{accountId}_{dto.TransactionId ?? Guid.NewGuid().ToString()}", // Composite ID
            AccountId = accountId,
            TransactionId = dto.TransactionId,
            Amount = decimal.Parse(dto.TransactionAmount.Amount, CultureInfo.InvariantCulture),
            Currency = dto.TransactionAmount.Currency,
            BookingDate = ParseDateTime(dto.BookingDate),
            ValueDate = ParseDateTime(dto.ValueDate),
            CreditorName = dto.CreditorName,
            DebtorName = dto.DebtorName,
            CreditorAccount = dto.CreditorAccount?.Iban,
            DebtorAccount = dto.DebtorAccount?.Iban,
            RemittanceInformationUnstructured = dto.RemittanceInformationUnstructured,
            BankTransactionCode = dto.BankTransactionCode,
            ProprietaryBankTransactionCode = dto.ProprietaryBankTransactionCode,
            EndToEndId = dto.EndToEndId,
            MandateId = dto.MandateId,
            CreditorId = dto.CreditorId,
            UltimateCreditor = dto.UltimateCreditor,
            UltimateDebtor = dto.UltimateDebtor,
            PurposeCode = dto.PurposeCode,
            AdditionalInformation = dto.AdditionalInformation,
            BalanceAfterTransaction =
                dto.BalanceAfterTransaction != null
                    ? decimal.Parse(dto.BalanceAfterTransaction.BalanceAmount.Amount, CultureInfo.InvariantCulture)
                    : null,
            IsPending = isPending,
        };

    private static DateTime? ParseDateTime(string? dateTimeString)
    {
        if (string.IsNullOrEmpty(dateTimeString))
        {
            return null;
        }

        // Try different date formats
        if (DateTime.TryParse(dateTimeString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
        {
            return result;
        }

        if (DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        {
            return result;
        }

        return null;
    }


    private static AccountLinkStatus AccountLinkStatusMapper(string status) =>
        status switch
        {
            "CR" => AccountLinkStatus.Pending,
            "GC" => AccountLinkStatus.Pending,
            "UA" => AccountLinkStatus.Pending,
            "RJ" => AccountLinkStatus.Failed,
            "SA" => AccountLinkStatus.Pending,
            "GA" => AccountLinkStatus.Pending,
            "LN" => AccountLinkStatus.Linked,
            "EX" => AccountLinkStatus.Expired,
            _ => throw new ArgumentOutOfRangeException(nameof(status), $"Unknown account link status: {status}"),
        };
}
