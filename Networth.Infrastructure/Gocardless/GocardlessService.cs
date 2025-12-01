using System.Globalization;
using System.Net;
using Microsoft.Extensions.Logging;
using Networth.Application.Interfaces;
using Networth.Domain.Entities;
using Networth.Domain.Enums;
using Networth.Infrastructure.Gocardless.DTOs;
using Refit;
using Account = Networth.Domain.Entities.Account;
using InstitutionMetadata = Networth.Domain.Entities.InstitutionMetadata;
using Requisition = Networth.Domain.Entities.Requisition;
using Transaction = Networth.Domain.Entities.Transaction;

namespace Networth.Infrastructure.Gocardless;

/// <summary>
///     GoCardless implementation of financial provider services.
/// </summary>
internal class GocardlessService(ILogger<GocardlessService> logger, IGocardlessClient gocardlessClient)
    : IFinancialProvider
{
    /// <inheritdoc />
    public async Task<InstitutionMetadata> GetInstitutionAsync(string institutionId, CancellationToken cancellationToken = default)
    {
        var response = await gocardlessClient.GetInstitution(institutionId, cancellationToken);
        EnsureSuccessStatusCode(response);
        GetInstitutionDto institution = response.Content!;
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
        var response = await gocardlessClient.GetInstitutions(country, cancellationToken);
        EnsureSuccessStatusCode(response);
        IEnumerable<GetInstitutionDto> dtos = response.Content!;

        var institutions = dtos.Select(dto =>
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
        logger.LogDebug(
            "Creating agreement for institution {InstitutionId}",
            institutionId);

        CreateAgreementRequestDto request = new()
        {
            InstitutionId = institutionId,
            MaxHistoricalDays = maxHistoricalDays,
            AccessValidForDays = accessValidForDays,
            AccessScope = [AccessScope.Details, AccessScope.Balances, AccessScope.Transactions],
        };

        var apiResponse = await gocardlessClient.CreateAgreement(request, cancellationToken);
        EnsureSuccessStatusCode(apiResponse);
        CreateAgreementResponseDto response = apiResponse.Content!;

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

    /// <inheritdoc />
    public async Task<Requisition> CreateRequisitionAsync(
        string institutionId,
        string agreementId,
        string redirectUrl,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug(
            "Creating requisition for institution {InstitutionId}",
            institutionId);

        CreateRequisitionRequestDto request = new()
        {
            Redirect = redirectUrl,
            InstitutionId = institutionId,
            Agreement = agreementId,
        };

        var apiResponse = await gocardlessClient.CreateRequisition(request, cancellationToken);
        EnsureSuccessStatusCode(apiResponse);
        CreateRequisitionResponseDto response = apiResponse.Content!;

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
    public async Task<Requisition?> GetRequisitionAsync(string requisitionId, CancellationToken cancellationToken = default)
    {
        var apiResponse = await gocardlessClient.GetRequisition(requisitionId, cancellationToken);

        if (IsNotFound(apiResponse))
        {
            logger.LogWarning("Requisition {RequisitionId} not found", requisitionId);
            return null;
        }

        EnsureSuccessStatusCode(apiResponse);
        GetRequisitionResponseDto response = apiResponse.Content!;

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
    public async Task<Account?> GetAccountAsync(string accountId, CancellationToken cancellationToken = default)
    {
        var apiResponse = await gocardlessClient.GetAccount(accountId, cancellationToken);

        if (IsNotFound(apiResponse))
        {
            logger.LogWarning("Account {AccountId} not found", accountId);
            return null;
        }

        EnsureSuccessStatusCode(apiResponse);
        GetAccountResponseDto response = apiResponse.Content!;

        return new Account
        {
            Id = response.Id,
            Status = response.Status,
            InstitutionId = response.InstitutionId,
            Name = response.Name,
        };
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AccountBalance>?> GetAccountBalancesAsync(
        string accountId,
        CancellationToken cancellationToken = default)
    {
        var apiResponse = await gocardlessClient.GetAccountBalances(accountId, cancellationToken);

        if (IsNotFound(apiResponse))
        {
            logger.LogWarning("Account {AccountId} not found", accountId);
            return null;
        }

        EnsureSuccessStatusCode(apiResponse);
        GetAccountBalanceResponseDto response = apiResponse.Content!;

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
    public async Task<AccountDetail?> GetAccountDetailsAsync(
        string accountId,
        CancellationToken cancellationToken = default)
    {
        var apiResponse = await gocardlessClient.GetAccountDetails(accountId, cancellationToken);

        if (IsNotFound(apiResponse))
        {
            logger.LogWarning("Account {AccountId} not found", accountId);
            return null;
        }

        EnsureSuccessStatusCode(apiResponse);
        GetAccountDetailResponseDto response = apiResponse.Content!;

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
    public async Task<IEnumerable<Transaction>?> GetAccountTransactionsAsync(
        string accountId,
        DateTimeOffset dateFrom,
        DateTimeOffset dateTo,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug(
            "Retrieving transactions for account {AccountId} from {DateFrom} to {DateTo}",
            accountId,
            dateFrom.ToString("yyyy-MM-dd"),
            dateTo.ToString("yyyy-MM-dd"));

        string? dateFromStr = dateFrom.UtcDateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        string? dateToStr = dateTo.UtcDateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        logger.LogInformation(
            "Calling GoCardless API with parameters: accountId={AccountId}, dateFrom={DateFrom}, dateTo={DateTo}",
            accountId,
            dateFromStr,
            dateToStr);

        var apiResponse =
            await gocardlessClient.GetAccountTransactions(accountId, dateFromStr, dateToStr, cancellationToken);

        logger.LogDebug(
            "GoCardless API response: StatusCode={StatusCode}, IsSuccessStatusCode={IsSuccess}",
            (int)apiResponse.StatusCode,
            apiResponse.IsSuccessStatusCode);

        if (IsNotFound(apiResponse))
        {
            logger.LogWarning("Account {AccountId} not found (404)", accountId);
            return null;
        }

        EnsureSuccessStatusCode(apiResponse);
        var response = apiResponse.Content!;

        List<Transaction> transactions = [];

        // Add booked transactions
        if (response.Transactions.Booked != null)
        {
            transactions.AddRange(response.Transactions.Booked.Select(transaction => MapTransactionDto(transaction, accountId, false)));
        }

        // Add pending transactions if available
        if (response.Transactions.Pending != null)
        {
            foreach (TransactionDto transaction in response.Transactions.Pending)
            {
                transactions.Add(MapTransactionDto(transaction, accountId, true));
            }
        }

        logger.LogDebug("Retrieved {TransactionCount} transactions for account {AccountId}", transactions.Count, accountId);

        return transactions;
    }

    private static Transaction MapTransactionDto(TransactionDto dto, string accountId, bool isPending) =>
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

    private static void EnsureSuccessStatusCode<T>(ApiResponse<T> response)
    {
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return; // Allow 404s to be handled by the caller
        }

        if (!response.IsSuccessStatusCode)
        {
            string errorDetails = response.Error?.Content ?? "No error details available";
            throw new HttpRequestException(
                $"GoCardless API returned status code {response.StatusCode}: {errorDetails}",
                null,
                response.StatusCode);
        }
    }

    private static bool IsNotFound<T>(ApiResponse<T> response) => response.StatusCode == HttpStatusCode.NotFound;
}
