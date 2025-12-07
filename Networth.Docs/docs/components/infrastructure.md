---
sidebar_position: 3
---

# Infrastructure Component

The `Networth.Infrastructure` project handles data access, external API integrations, and infrastructure concerns.

## Overview

**Purpose**: Data persistence and external service integration
**Technologies**: EF Core, Refit, Azure Storage Queues
**Location**: `/Networth.Infrastructure`

## Project Structure

```
Networth.Infrastructure/
├── Data/
│   ├── Context/
│   │   └── NetworthDbContext.cs
│   ├── Entities/          # EF Core entities
│   │   ├── AccountEntity.cs
│   │   ├── TransactionEntity.cs
│   │   └── ...
│   ├── Configurations/    # EF configurations
│   │   ├── AccountConfiguration.cs
│   │   └── ...
│   └── Repositories/      # Repository implementations
│       ├── AccountRepository.cs
│       └── ...
├── ExternalServices/
│   ├── GoCardless/
│   │   ├── IGocardlessClient.cs
│   │   ├── GocardlessService.cs
│   │   ├── GoCardlessAuthHandler.cs
│   │   └── Models/
│   └── Queue/
│       └── QueueService.cs
└── Extensions/
    └── ServiceCollectionExtensions.cs
```

## Database Access (EF Core)

### DbContext

```csharp
public class NetworthDbContext : DbContext
{
    public NetworthDbContext(DbContextOptions<NetworthDbContext> options)
        : base(options)
    {
    }

    public DbSet<AccountEntity> Accounts { get; set; }
    public DbSet<TransactionEntity> Transactions { get; set; }
    public DbSet<AccountBalanceEntity> AccountBalances { get; set; }
    public DbSet<RequisitionEntity> Requisitions { get; set; }
    public DbSet<AgreementEntity> Agreements { get; set; }
    public DbSet<InstitutionEntity> Institutions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NetworthDbContext).Assembly);
    }
}
```

### Entity Configuration

```csharp
public class AccountConfiguration : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(a => a.InstitutionId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.Name)
            .HasMaxLength(200);

        builder.HasIndex(a => new { a.UserId, a.InstitutionId });

        // Relationships
        builder.HasMany(a => a.Balances)
            .WithOne(b => b.Account)
            .HasForeignKey(b => b.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Transactions)
            .WithOne(t => t.Account)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### Repository Pattern

#### Interface (in Domain)

```csharp
public interface IAccountRepository
{
    Task<IReadOnlyList<Account>> GetAccountsByUserIdAsync(
        Guid userId,
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
}
```

#### Implementation (in Infrastructure)

```csharp
public class AccountRepository : IAccountRepository
{
    private readonly NetworthDbContext _context;

    public AccountRepository(NetworthDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Account>> GetAccountsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Accounts
            .Where(a => a.UserId == userId)
            .Include(a => a.Balances)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToDomain).ToList();
    }

    public async Task<Account?> GetByIdAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Accounts
            .Include(a => a.Balances)
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);

        return entity != null ? MapToDomain(entity) : null;
    }

    public async Task CreateAsync(
        Account account,
        CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(account);
        await _context.Accounts.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Account account,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Accounts
            .FindAsync(new object[] { account.Id }, cancellationToken);

        if (entity == null)
            throw new InvalidOperationException($"Account {account.Id} not found");

        UpdateEntity(entity, account);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private Account MapToDomain(AccountEntity entity)
    {
        return new Account(
            entity.Id,
            entity.InstitutionId,
            entity.UserId,
            entity.Name,
            entity.Status
        );
    }

    private AccountEntity MapToEntity(Account domain)
    {
        return new AccountEntity
        {
            Id = domain.Id,
            InstitutionId = domain.InstitutionId,
            UserId = domain.UserId,
            Name = domain.Name,
            Status = domain.Status
        };
    }
}
```

## GoCardless API Integration

### Refit Client Interface

```csharp
public interface IGocardlessClient
{
    [Get("/api/v2/institutions?country={country}")]
    Task<InstitutionsResponse> GetInstitutionsAsync(
        string country,
        CancellationToken cancellationToken = default);

    [Post("/api/v2/agreements")]
    Task<AgreementResponse> CreateAgreementAsync(
        [Body] CreateAgreementRequest request,
        CancellationToken cancellationToken = default);

    [Post("/api/v2/requisitions")]
    Task<RequisitionResponse> CreateRequisitionAsync(
        [Body] CreateRequisitionRequest request,
        CancellationToken cancellationToken = default);

    [Get("/api/v2/requisitions/{id}")]
    Task<RequisitionResponse> GetRequisitionAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    [Get("/api/v2/accounts/{id}/balances")]
    Task<BalancesResponse> GetAccountBalancesAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    [Get("/api/v2/accounts/{id}/details")]
    Task<AccountDetailsResponse> GetAccountDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    [Get("/api/v2/accounts/{id}/transactions")]
    Task<TransactionsResponse> GetAccountTransactionsAsync(
        Guid id,
        [Query] string date_from,
        [Query] string date_to,
        CancellationToken cancellationToken = default);
}
```

### Service Layer

```csharp
public interface IGocardlessService
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
}

public class GocardlessService : IGocardlessService
{
    private readonly IGocardlessClient _client;

    public GocardlessService(IGocardlessClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<Institution>> GetInstitutionsAsync(
        string country,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.GetInstitutionsAsync(country, cancellationToken);
        return response.Institutions.Select(MapToDomain).ToList();
    }

    public async Task<Agreement> CreateAgreementAsync(
        string institutionId,
        CancellationToken cancellationToken = default)
    {
        var request = new CreateAgreementRequest
        {
            InstitutionId = institutionId,
            MaxHistoricalDays = 90,
            AccessValidForDays = 90,
            AccessScope = new[] { "balances", "details", "transactions" },
            Reconfirmation = true
        };

        var response = await _client.CreateAgreementAsync(request, cancellationToken);
        return MapToDomain(response);
    }
}
```

### Authentication Handler

```csharp
public class GoCardlessAuthHandler : DelegatingHandler
{
    private readonly IGoCardlessTokenManager _tokenManager;

    public GoCardlessAuthHandler(IGoCardlessTokenManager tokenManager)
    {
        _tokenManager = tokenManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenManager.GetTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
```

### Retry Handler

```csharp
public class RefitRetryHandler : DelegatingHandler
{
    private const int MaxRetries = 3;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        for (int i = 0; i < MaxRetries; i++)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode || 
                    response.StatusCode == HttpStatusCode.NotFound)
                {
                    return response;
                }

                if (i == MaxRetries - 1)
                    return response;

                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)), cancellationToken);
            }
            catch (HttpRequestException) when (i < MaxRetries - 1)
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)), cancellationToken);
            }
        }

        throw new InvalidOperationException("Max retries exceeded");
    }
}
```

## Azure Queue Service

```csharp
public interface IQueueService
{
    Task EnqueueAsync<T>(string queueName, T message, CancellationToken cancellationToken = default);
}

public class QueueService : IQueueService
{
    private readonly QueueServiceClient _queueServiceClient;

    public QueueService(QueueServiceClient queueServiceClient)
    {
        _queueServiceClient = queueServiceClient;
    }

    public async Task EnqueueAsync<T>(
        string queueName,
        T message,
        CancellationToken cancellationToken = default)
    {
        var queueClient = _queueServiceClient.GetQueueClient(queueName);
        await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        var base64 = Convert.ToBase64String(bytes);

        await queueClient.SendMessageAsync(base64, cancellationToken: cancellationToken);
    }
}
```

## Service Registration

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<NetworthDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("NetworthDb")));

        // Repositories
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        // GoCardless
        services.AddSingleton<IGoCardlessTokenManager, GoCardlessTokenManager>();
        services.AddTransient<GoCardlessAuthHandler>();
        services.AddTransient<RefitRetryHandler>();

        services.AddRefitClient<IGocardlessClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://bankaccountdata.gocardless.com"))
            .AddHttpMessageHandler<RefitRetryHandler>()
            .AddHttpMessageHandler<GoCardlessAuthHandler>();

        services.AddScoped<IGocardlessService, GocardlessService>();

        // Queue Service
        services.AddSingleton(sp =>
        {
            var connectionString = configuration.GetConnectionString("queues");
            return new QueueServiceClient(connectionString);
        });
        services.AddScoped<IQueueService, QueueService>();

        return services;
    }
}
```

## Database Initialization

```csharp
public static class DatabaseExtensions
{
    public static async Task EnsureDatabaseCreatedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NetworthDbContext>();

        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Test"))
        {
            await context.Database.EnsureCreatedAsync();
        }
    }
}
```
