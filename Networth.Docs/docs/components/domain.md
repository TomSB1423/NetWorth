---
sidebar_position: 4
---

# Domain Component

The `Networth.Domain` project contains the core business entities and contracts, independent of infrastructure concerns.

## Overview

**Purpose**: Core business logic and domain models
**Pattern**: Domain-Driven Design (DDD) principles
**Location**: `/Networth.Domain`

## Project Structure

```
Networth.Domain/
├── Entities/              # Domain entities
│   ├── Account.cs
│   ├── Transaction.cs
│   ├── AccountBalance.cs
│   ├── Institution.cs
│   ├── Agreement.cs
│   └── Requisition.cs
├── Repositories/          # Repository interfaces
│   ├── IAccountRepository.cs
│   ├── ITransactionRepository.cs
│   └── ...
└── Services/              # Domain service interfaces
    └── IFinancialProvider.cs
```

## Domain Entities

Domain entities are **immutable records** that represent core business concepts.

### Account

```csharp
public record Account(
    Guid Id,
    string InstitutionId,
    Guid UserId,
    string? Name,
    string Status
)
{
    public static Account Create(
        string institutionId,
        Guid userId,
        string? name = null)
    {
        return new Account(
            Id: Guid.NewGuid(),
            InstitutionId: institutionId,
            UserId: userId,
            Name: name,
            Status: "active"
        );
    }
}
```

### Transaction

```csharp
public record Transaction(
    Guid Id,
    Guid AccountId,
    string TransactionId,
    string? DebtorName,
    string? DebtorAccountIban,
    decimal Amount,
    string Currency,
    string? BankTransactionCode,
    DateOnly BookingDate,
    DateOnly ValueDate,
    string? RemittanceInformation,
    string Status
)
{
    public bool IsBooked => Status == "booked";
    public bool IsPending => Status == "pending";
}
```

### AccountBalance

```csharp
public record AccountBalance(
    Guid Id,
    Guid AccountId,
    decimal Amount,
    string Currency,
    string BalanceType,
    DateOnly ReferenceDate
)
{
    public bool IsInterimAvailable => BalanceType == "interimAvailable";
    public bool IsExpected => BalanceType == "expected";
}
```

### Institution

```csharp
public record Institution(
    string Id,
    string Name,
    string Bic,
    string LogoUrl,
    int TransactionTotalDays,
    int MaxAccessValidForDays,
    IReadOnlyList<string> Countries,
    IReadOnlyList<string> SupportedFeatures
)
{
    public bool SupportsFeature(string feature)
    {
        return SupportedFeatures.Contains(feature);
    }

    public bool OperatesInCountry(string countryCode)
    {
        return Countries.Contains(countryCode, StringComparer.OrdinalIgnoreCase);
    }
}
```

### Agreement

```csharp
public record Agreement(
    Guid Id,
    string InstitutionId,
    int MaxHistoricalDays,
    int AccessValidForDays,
    IReadOnlyList<string> AccessScope,
    bool Reconfirmation,
    DateTime Created,
    DateTime? Accepted
)
{
    public bool IsAccepted => Accepted.HasValue;

    public bool IsExpired(DateTime now)
    {
        if (!Accepted.HasValue) return false;
        return now > Accepted.Value.AddDays(AccessValidForDays);
    }
}
```

### Requisition

```csharp
public record Requisition(
    Guid Id,
    string InstitutionId,
    Guid AgreementId,
    string Status,
    string RedirectUrl,
    string? Link,
    IReadOnlyList<Guid> Accounts,
    DateTime Created
)
{
    public bool IsCreated => Status == "CR";
    public bool IsLinked => Status == "LN";
    public bool IsExpired => Status == "EX";
    public bool IsSuspended => Status == "SU";
    public bool IsRejected => Status == "RJ";

    public bool HasAccounts => Accounts.Any();
}
```

## Repository Interfaces

Repository interfaces define data access contracts without implementation details.

### IAccountRepository

```csharp
public interface IAccountRepository
{
    Task<IReadOnlyList<Account>> GetAccountsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Account>> GetAccountsByInstitutionAsync(
        Guid userId,
        string institutionId,
        CancellationToken cancellationToken = default);

    Task<Account?> GetByIdAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        Account account,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Account account,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);
}
```

### ITransactionRepository

```csharp
public interface ITransactionRepository
{
    Task<IReadOnlyList<Transaction>> GetByAccountIdAsync(
        Guid accountId,
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default);

    Task<Transaction?> GetByTransactionIdAsync(
        string transactionId,
        CancellationToken cancellationToken = default);

    Task UpsertAsync(
        IReadOnlyList<Transaction> transactions,
        CancellationToken cancellationToken = default);
}
```

### IAccountBalanceRepository

```csharp
public interface IAccountBalanceRepository
{
    Task<IReadOnlyList<AccountBalance>> GetByAccountIdAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    Task UpsertAsync(
        IReadOnlyList<AccountBalance> balances,
        CancellationToken cancellationToken = default);
}
```

### IInstitutionRepository

```csharp
public interface IInstitutionRepository
{
    Task<IReadOnlyList<Institution>> GetByCountryAsync(
        string country,
        CancellationToken cancellationToken = default);

    Task<Institution?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task UpsertAsync(
        IReadOnlyList<Institution> institutions,
        CancellationToken cancellationToken = default);
}
```

### IAgreementRepository

```csharp
public interface IAgreementRepository
{
    Task<Agreement?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Agreement>> GetByInstitutionIdAsync(
        string institutionId,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        Agreement agreement,
        CancellationToken cancellationToken = default);
}
```

### IRequisitionRepository

```csharp
public interface IRequisitionRepository
{
    Task<Requisition?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Requisition>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        Requisition requisition,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Requisition requisition,
        CancellationToken cancellationToken = default);
}
```

## Domain Services

Domain services encapsulate business logic that doesn't belong to a single entity.

### IFinancialProvider

```csharp
public interface IFinancialProvider
{
    Task<IReadOnlyList<Institution>> GetInstitutionsAsync(
        string country,
        CancellationToken cancellationToken = default);

    Task<Agreement> CreateAgreementAsync(
        string institutionId,
        CancellationToken cancellationToken = default);

    Task<Requisition> CreateRequisitionAsync(
        string institutionId,
        Guid agreementId,
        string redirectUrl,
        CancellationToken cancellationToken = default);

    Task<Requisition> GetRequisitionAsync(
        Guid requisitionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AccountBalance>> GetAccountBalancesAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    Task<AccountDetails> GetAccountDetailsAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Transaction>> GetAccountTransactionsAsync(
        Guid accountId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);
}
```

## Design Principles

### 1. Immutability

Domain entities are immutable records:
- Easier to reason about
- Thread-safe by default
- Prevent accidental state changes

### 2. Rich Domain Models

Entities contain behavior, not just data:

```csharp
public record Agreement(...)
{
    public bool IsExpired(DateTime now)
    {
        if (!Accepted.HasValue) return false;
        return now > Accepted.Value.AddDays(AccessValidForDays);
    }
}
```

### 3. Dependency Inversion

Domain depends on abstractions (interfaces), not concrete implementations:
- Infrastructure implements domain interfaces
- Domain doesn't reference Infrastructure

### 4. Ubiquitous Language

Entity and property names reflect business terminology:
- `Requisition` (not `LinkRequest`)
- `InterimAvailable` balance type
- `Booked` vs `Pending` transactions

### 5. Factory Methods

Static factory methods for entity creation:

```csharp
public record Account(...)
{
    public static Account Create(string institutionId, Guid userId, string? name = null)
    {
        return new Account(
            Id: Guid.NewGuid(),
            InstitutionId: institutionId,
            UserId: userId,
            Name: name,
            Status: "active"
        );
    }
}
```

## Value Objects

Simple value types represented as records:

```csharp
public record Money(decimal Amount, string Currency)
{
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");

        return this with { Amount = Amount + other.Amount };
    }
}
```

## No Dependencies

The Domain project has **zero dependencies** on:
- Infrastructure
- Application
- External libraries (except maybe basic utilities)

This keeps the core business logic pure and testable.
