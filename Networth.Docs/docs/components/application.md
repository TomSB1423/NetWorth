---
sidebar_position: 2
---

# Application Component

The `Networth.Application` project contains the business logic layer using a custom mediator pattern for command and query handling.

## Overview

**Purpose**: Business logic and use case orchestration
**Pattern**: CQRS-lite with custom mediator
**Location**: `/Networth.Application`

## Project Structure

```
Networth.Application/
├── Commands/           # Write operations
│   ├── LinkAccountCommand.cs
│   └── SyncAccountCommand.cs
├── Queries/           # Read operations
│   ├── GetAccountsQuery.cs
│   ├── GetInstitutionsQuery.cs
│   └── GetRequisitionQuery.cs
├── Handlers/          # Command and Query handlers
│   ├── LinkAccountCommandHandler.cs
│   ├── GetAccountsQueryHandler.cs
│   └── ...
├── Validators/        # FluentValidation validators
│   ├── LinkAccountCommandValidator.cs
│   └── ...
├── Services/          # Application services
│   ├── Mediator.cs
│   └── ...
└── Extensions/
    └── ServiceCollectionExtensions.cs
```

## Custom Mediator Pattern

The application uses a **custom, lightweight mediator** (not MediatR).

### Request Interface

```csharp
public interface IRequest<TResponse>
{
}

// Example query
public record GetAccountsQuery(Guid UserId) : IRequest<GetAccountsQueryResult>;

// Example command
public record LinkAccountCommand(
    string InstitutionId,
    Guid UserId,
    string RedirectUrl
) : IRequest<LinkAccountCommandResult>;
```

### Handler Interface

```csharp
public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

// Example handler
public class GetAccountsQueryHandler 
    : IRequestHandler<GetAccountsQuery, GetAccountsQueryResult>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountsQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<GetAccountsQueryResult> Handle(
        GetAccountsQuery request,
        CancellationToken cancellationToken)
    {
        var accounts = await _accountRepository
            .GetAccountsByUserIdAsync(request.UserId, cancellationToken);

        return new GetAccountsQueryResult(accounts);
    }
}
```

### Mediator Implementation

```csharp
public interface IMediator
{
    Task<TResponse> Send<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>;
}

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        // Get validator if exists
        var validator = _serviceProvider
            .GetService<IValidator<TRequest>>();
        
        if (validator != null)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }

        // Get and execute handler
        var handler = _serviceProvider
            .GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        
        return await handler.Handle(request, cancellationToken);
    }
}
```

## Commands

Commands represent write operations that change system state.

### LinkAccountCommand

```csharp
public record LinkAccountCommand(
    string InstitutionId,
    Guid UserId,
    string RedirectUrl
) : IRequest<LinkAccountCommandResult>;

public record LinkAccountCommandResult(
    Guid RequisitionId,
    string AuthLink
);

public class LinkAccountCommandHandler 
    : IRequestHandler<LinkAccountCommand, LinkAccountCommandResult>
{
    private readonly IGocardlessService _gocardlessService;
    private readonly IAgreementRepository _agreementRepository;
    private readonly IRequisitionRepository _requisitionRepository;

    public async Task<LinkAccountCommandResult> Handle(
        LinkAccountCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Create agreement
        var agreement = await _gocardlessService.CreateAgreementAsync(
            request.InstitutionId,
            cancellationToken
        );
        await _agreementRepository.CreateAsync(agreement, cancellationToken);

        // 2. Create requisition
        var requisition = await _gocardlessService.CreateRequisitionAsync(
            request.InstitutionId,
            agreement.Id,
            request.RedirectUrl,
            cancellationToken
        );
        await _requisitionRepository.CreateAsync(requisition, cancellationToken);

        return new LinkAccountCommandResult(requisition.Id, requisition.Link);
    }
}
```

### SyncAccountCommand

```csharp
public record SyncAccountCommand(
    Guid AccountId,
    DateOnly DateFrom,
    DateOnly DateTo
) : IRequest<Unit>;

public class SyncAccountCommandHandler 
    : IRequestHandler<SyncAccountCommand, Unit>
{
    private readonly IGocardlessService _gocardlessService;
    private readonly IAccountBalanceRepository _balanceRepository;
    private readonly ITransactionRepository _transactionRepository;

    public async Task<Unit> Handle(
        SyncAccountCommand request,
        CancellationToken cancellationToken)
    {
        // Fetch and store balances
        var balances = await _gocardlessService.GetAccountBalancesAsync(
            request.AccountId,
            cancellationToken
        );
        await _balanceRepository.UpsertAsync(balances, cancellationToken);

        // Fetch and store transactions
        var transactions = await _gocardlessService.GetAccountTransactionsAsync(
            request.AccountId,
            request.DateFrom,
            request.DateTo,
            cancellationToken
        );
        await _transactionRepository.UpsertAsync(transactions, cancellationToken);

        return Unit.Value;
    }
}
```

## Queries

Queries represent read operations that don't change state.

### GetAccountsQuery

```csharp
public record GetAccountsQuery(Guid UserId) : IRequest<GetAccountsQueryResult>;

public record GetAccountsQueryResult(IReadOnlyList<Account> Accounts);

public class GetAccountsQueryHandler 
    : IRequestHandler<GetAccountsQuery, GetAccountsQueryResult>
{
    private readonly IAccountRepository _accountRepository;

    public async Task<GetAccountsQueryResult> Handle(
        GetAccountsQuery request,
        CancellationToken cancellationToken)
    {
        var accounts = await _accountRepository
            .GetAccountsByUserIdAsync(request.UserId, cancellationToken);

        return new GetAccountsQueryResult(accounts);
    }
}
```

### GetInstitutionsQuery

```csharp
public record GetInstitutionsQuery(string Country) : IRequest<GetInstitutionsQueryResult>;

public record GetInstitutionsQueryResult(IReadOnlyList<Institution> Institutions);

public class GetInstitutionsQueryHandler 
    : IRequestHandler<GetInstitutionsQuery, GetInstitutionsQueryResult>
{
    private readonly IInstitutionRepository _institutionRepository;
    private readonly IGocardlessService _gocardlessService;

    public async Task<GetInstitutionsQueryResult> Handle(
        GetInstitutionsQuery request,
        CancellationToken cancellationToken)
    {
        // Check cache first
        var cached = await _institutionRepository
            .GetByCountryAsync(request.Country, cancellationToken);

        if (cached?.Any() == true)
        {
            return new GetInstitutionsQueryResult(cached);
        }

        // Fetch from API
        var institutions = await _gocardlessService
            .GetInstitutionsAsync(request.Country, cancellationToken);

        // Cache for future requests
        await _institutionRepository.UpsertAsync(institutions, cancellationToken);

        return new GetInstitutionsQueryResult(institutions);
    }
}
```

## Validation

FluentValidation is used for request validation:

```csharp
public class LinkAccountCommandValidator : AbstractValidator<LinkAccountCommand>
{
    public LinkAccountCommandValidator()
    {
        RuleFor(x => x.InstitutionId)
            .NotEmpty()
            .WithMessage("Institution ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.RedirectUrl)
            .NotEmpty()
            .Must(BeAValidUrl)
            .WithMessage("Redirect URL must be a valid URL");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
```

## Service Registration

All handlers and validators are registered in `ServiceCollectionExtensions`:

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register mediator
        services.AddScoped<IMediator, Mediator>();

        // Register handlers
        services.AddScoped<
            IRequestHandler<GetAccountsQuery, GetAccountsQueryResult>,
            GetAccountsQueryHandler>();

        services.AddScoped<
            IRequestHandler<LinkAccountCommand, LinkAccountCommandResult>,
            LinkAccountCommandHandler>();

        // Register validators
        services.AddScoped<
            IValidator<LinkAccountCommand>,
            LinkAccountCommandValidator>();

        services.AddScoped<
            IValidator<GetAccountsQuery>,
            GetAccountsQueryValidator>();

        return services;
    }
}
```

## Unit Type

For commands that don't return a value, use `Unit`:

```csharp
public record SyncAccountCommand(...) : IRequest<Unit>;

public class SyncAccountCommandHandler : IRequestHandler<SyncAccountCommand, Unit>
{
    public async Task<Unit> Handle(...)
    {
        // Do work
        return Unit.Value;
    }
}

public struct Unit
{
    public static Unit Value => default;
}
```

## Best Practices

1. **Immutable Records**: Use records for commands and queries
2. **Single Responsibility**: Each handler does one thing
3. **Validate Early**: Use validators to catch bad input
4. **Thin Handlers**: Delegate complex logic to domain services
5. **Async All the Way**: Use async/await for all I/O operations
6. **Descriptive Names**: Use clear, action-oriented names
