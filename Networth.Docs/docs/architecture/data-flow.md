---
sidebar_position: 5
---

# Data Flow

Understanding how data flows through the Networth application is crucial for development and troubleshooting. This page describes the data flow for key user journeys.

## Account Linking Flow

```mermaid
sequenceDiagram
    participant User
    participant Frontend
    participant Functions
    participant Handler
    participant GoCardless
    participant Database

    User->>Frontend: Select institution
    Frontend->>Functions: POST /api/accounts/link
    Functions->>Handler: LinkAccountCommandHandler
    
    Note over Handler,GoCardless: Create Agreement
    Handler->>GoCardless: POST /api/v2/agreements
    GoCardless-->>Handler: Agreement ID
    Handler->>Database: Store agreement
    
    Note over Handler,GoCardless: Create Requisition
    Handler->>GoCardless: POST /api/v2/requisitions
    GoCardless-->>Handler: Requisition with auth link
    Handler->>Database: Store requisition
    Handler-->>Functions: Return auth link
    Functions-->>Frontend: Requisition + link
    
    Frontend->>User: Redirect to GoCardless
    User->>GoCardless: Authenticate & authorize
    GoCardless-->>Frontend: Redirect with requisition ID
    
    Frontend->>Functions: GET /api/requisitions/{id}
    Functions->>Handler: GetRequisitionQueryHandler
    Handler->>GoCardless: GET /api/v2/requisitions/{id}
    GoCardless-->>Handler: Requisition (status: LN, accounts: [ids])
    Handler->>Database: Update requisition, store account IDs
    Handler-->>Functions: Requisition with accounts
    Functions-->>Frontend: Account list
    Frontend->>User: Show linked accounts
```

### Steps Explained

1. **User Selection**: User selects a financial institution
2. **Agreement Creation**: Handler creates access agreement with GoCardless
3. **Requisition Creation**: Handler creates requisition for user authorization
4. **User Authorization**: User authenticates with their bank via GoCardless
5. **Account Storage**: System retrieves and stores linked account IDs
6. **Confirmation**: User sees their newly linked accounts

## Account Synchronization Flow

```mermaid
sequenceDiagram
    participant User
    participant Frontend
    participant HTTP as HTTP Function
    participant Queue as Azure Queue
    participant QueueFunc as Queue Function
    participant GoCardless
    participant Database

    User->>Frontend: Click "Sync Accounts"
    Frontend->>HTTP: POST /api/institutions/{id}/sync
    
    HTTP->>Database: Get user's accounts for institution
    Database-->>HTTP: Account list
    
    loop For each account
        HTTP->>Queue: Enqueue sync message
    end
    
    HTTP-->>Frontend: Sync initiated
    Frontend->>User: Show "Syncing..."
    
    Note over Queue,QueueFunc: Background Processing
    Queue->>QueueFunc: Trigger SyncAccount
    
    QueueFunc->>GoCardless: GET /api/v2/accounts/{id}/balances
    GoCardless-->>QueueFunc: Current balances
    QueueFunc->>Database: Cache balances with timestamp
    
    QueueFunc->>GoCardless: GET /api/v2/accounts/{id}/details
    GoCardless-->>QueueFunc: Account details
    QueueFunc->>Database: Cache account details
    
    QueueFunc->>GoCardless: GET /api/v2/accounts/{id}/transactions
    GoCardless-->>QueueFunc: Transactions (booked + pending)
    QueueFunc->>Database: Upsert transactions
    
    QueueFunc->>Database: Log sync completion
    
    Note over Frontend,User: Polling or Real-time Update
    Frontend->>HTTP: GET /api/accounts/{id}/balances
    HTTP->>Database: Query cached data
    Database-->>HTTP: Latest balances
    HTTP-->>Frontend: Updated data
    Frontend->>User: Show updated balances
```

### Steps Explained

1. **Sync Trigger**: User initiates sync for an institution
2. **Job Queuing**: HTTP function enqueues sync job for each account
3. **Background Processing**: Queue function processes each account
4. **Data Fetching**: Fetch balances, details, and transactions from GoCardless
5. **Data Storage**: Cache all fetched data in PostgreSQL
6. **User Update**: Frontend polls or receives updates to show fresh data

## Data Read Flow

```mermaid
sequenceDiagram
    participant User
    participant Frontend
    participant Functions
    participant Handler
    participant Repository
    participant Database

    User->>Frontend: View account details
    Frontend->>Functions: GET /api/accounts/{id}/balances
    Functions->>Handler: GetAccountBalancesQueryHandler
    Handler->>Repository: GetAccountBalancesAsync(accountId)
    Repository->>Database: SELECT * FROM AccountBalances
    Database-->>Repository: Balance records
    Repository-->>Handler: Domain entities
    Handler-->>Functions: Query result
    Functions-->>Frontend: JSON response
    Frontend->>User: Display balances
```

### Caching Strategy

- **Balances**: Cached with timestamp, stale after 1 hour
- **Details**: Cached indefinitely (rarely changes)
- **Transactions**: Cached per date range, refreshed on sync

## Institution Discovery Flow

```mermaid
sequenceDiagram
    participant User
    participant Frontend
    participant Functions
    participant Handler
    participant Database
    participant GoCardless

    User->>Frontend: Select country
    Frontend->>Functions: GET /api/institutions?country=GB
    Functions->>Handler: GetInstitutionsQueryHandler
    
    Handler->>Database: Check cache for institutions
    
    alt Cache hit (valid)
        Database-->>Handler: Cached institutions
    else Cache miss or stale
        Handler->>GoCardless: GET /api/v2/institutions?country=GB
        GoCardless-->>Handler: Institution list
        Handler->>Database: Cache institutions
    end
    
    Handler-->>Functions: Institution list
    Functions-->>Frontend: JSON response
    Frontend->>User: Display institutions
```

## Data Models

### Domain Entities (Read-Only from API)

```csharp
public record Institution(
    string Id,
    string Name,
    string Bic,
    string LogoUrl,
    IReadOnlyList<string> Countries,
    IReadOnlyList<string> SupportedFeatures
);

public record AccountBalance(
    decimal Amount,
    string Currency,
    string BalanceType,
    DateOnly ReferenceDate
);

public record Transaction(
    string TransactionId,
    string DebtorName,
    string DebtorAccountIban,
    decimal Amount,
    string Currency,
    DateOnly BookingDate,
    DateOnly ValueDate,
    string RemittanceInformation,
    string Status
);
```

### Infrastructure Entities (EF Core)

```csharp
public class AccountBalanceEntity
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string BalanceType { get; set; } = string.Empty;
    public DateOnly ReferenceDate { get; set; }
    public DateTime CachedAt { get; set; }
    
    // Navigation properties
    public AccountEntity Account { get; set; } = null!;
}
```

## Error Handling Flow

```mermaid
sequenceDiagram
    participant User
    participant Frontend
    participant Functions
    participant Handler
    participant External as External API

    User->>Frontend: Perform action
    Frontend->>Functions: API Request
    Functions->>Handler: Process request
    Handler->>External: External API call
    External-->>Handler: Error (404, 500, etc.)
    Handler-->>Functions: Throw exception
    Functions->>Functions: Exception middleware catches
    Functions-->>Frontend: Structured error response
    Frontend->>User: Display friendly error message
```

### Error Categories

1. **Validation Errors**: FluentValidation catches invalid requests
2. **Not Found**: Resource doesn't exist (404)
3. **External API Errors**: GoCardless API failures (retried automatically)
4. **Database Errors**: Connection or query issues
5. **Business Logic Errors**: Domain rule violations

## Performance Considerations

### Database Queries

- **Indexes**: On frequently queried columns (accountId, userId)
- **Batch Operations**: Bulk insert for transactions
- **Connection Pooling**: Configured in EF Core

### API Rate Limiting

- **GoCardless**: Implements retry with exponential backoff
- **Queue Processing**: Prevents overwhelming external API

### Frontend Caching

- **React Query**: 5-minute stale time for most data
- **Optimistic Updates**: UI updates before API confirmation
- **Background Refetch**: Fresh data without blocking UI
