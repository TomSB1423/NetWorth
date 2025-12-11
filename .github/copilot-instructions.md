# Networth Application - AI Coding Agent Instructions

## Agent Tips

-   Use context7 MCP server for obtaining documentation and codebase context.
-   Use Playwright to interact with the browser-based parts of the application if needed.

---

## 1. Monorepo Overview

**.NET Aspire monorepo** - personal finance aggregation app syncing bank accounts via GoCardless API.

```
Networth/
├── Networth.AppHost/        # 🎯 Aspire orchestrator (start here)
├── Networth.Functions/      # ⚡ Azure Functions API (HTTP + Queue triggers)
├── Networth.Application/    # 📦 Business logic (custom mediator, CQRS)
├── Networth.Infrastructure/ # 🔌 EF Core + GoCardless Refit client
├── Networth.Domain/         # 📐 Domain entities + repository interfaces
├── Networth.Frontend/       # ⚛️ React SPA (Vite + TanStack Query + Tailwind)
├── Networth.ServiceDefaults/# 🔧 Shared Aspire config + resource constants
├── Networth.Docs/           # 📚 Docusaurus documentation site
└── Tests/                   # 🧪 Integration tests (Aspire.Hosting.Testing)
```

**Run everything**: `dotnet run --project Networth.AppHost`

**Required secrets**:

```bash
dotnet user-secrets set "Gocardless:SecretId" "YOUR_ID" --project Networth.AppHost
dotnet user-secrets set "Gocardless:SecretKey" "YOUR_KEY" --project Networth.AppHost
```

---

## 2. Architecture Layers

### AppHost (Orchestration)

`AppHost.cs` wires up: PostgreSQL, Azurite queues, Azure Functions, React frontend, Docusaurus docs. All resource names defined in `ResourceNames.cs`.

### Functions (API Layer)

-   **HTTP endpoints**: `Functions/Http/{Resource}/{Action}.cs` - thin controllers calling mediator
-   **Queue triggers**: `Functions/Queues/` - background processing (account sync, balance calculation)
-   Uses `ICurrentUserService` for auth context

### Application (Business Logic)

-   **Custom mediator** (NOT MediatR) - see `Services/Mediator.cs`
-   Commands/Queries in separate folders, handlers implement `IRequestHandler<TRequest, TResponse>`
-   FluentValidation validators auto-registered
-   Registration in `Extensions/ServiceCollectionExtensions.cs`

### Infrastructure (Data + External)

-   **EF Core**: `Data/Context/NetworthDbContext.cs`, entity configs in `Data/Configurations/`
-   **GoCardless**: Refit client `IGocardlessClient.cs` with snake_case JSON, auto token refresh
-   Repository implementations in `Data/Repositories/`

### Domain

-   Read-only record types representing external API responses
-   Repository interfaces (implemented in Infrastructure)

### Frontend

-   Vite + React + TypeScript + TanStack Query + Tailwind
-   API calls via `services/api.ts`, shared state via `contexts/AccountContext.tsx`
-   `VITE_API_URL` env var injected by Aspire

---

## 3. Key Conventions

| Convention                     | Details                                                                  |
| ------------------------------ | ------------------------------------------------------------------------ |
| **Central Package Management** | Versions in `Directory.Packages.props`, no versions in `.csproj`         |
| **Code Quality**               | `TreatWarningsAsErrors: true`, nullable enabled, XML docs required       |
| **Resource Names**             | Use constants from `ResourceNames.cs` for all Aspire resources           |
| **Entity Distinction**         | `Domain.Entities` = API DTOs; `Infrastructure.Data.Entities` = DB models |
| **Queue Names**                | `AccountSyncQueue`, `CalculateRunningBalanceQueue` in `ResourceNames.cs` |

---

## 4. Testing

```bash
dotnet test
```

-   `DistributedApplicationTestFactory` spins up full Aspire stack
-   `MockoonTestFixture` mocks GoCardless API
-   Random volume names for test isolation

---

## 5. Adding Features

| Task               | Key Files                                                                                          |
| ------------------ | -------------------------------------------------------------------------------------------------- |
| **API endpoint**   | `Application/{Queries,Commands}/` → Handler → `ServiceCollectionExtensions.cs` → `Functions/Http/` |
| **Background job** | Add queue to `ResourceNames` → Command+Handler → `Functions/Queues/`                               |
| **DB entity**      | `Infrastructure/Data/Entities/` → DbContext → Configuration → Repository                           |
| **Frontend page**  | `src/pages/` → Route in `App.tsx` → TanStack Query for data                                        |

---

## 6. Domain Concepts

### Core Entities

-   **User** - Application user who owns accounts
-   **Account** - Bank account linked via GoCardless (checking, savings, credit, etc.)
-   **Transaction** - Individual financial transaction with amount, date, counterparty
-   **Balance** - Point-in-time account balance (booked, available, pending)
-   **Institution** - Bank/financial institution metadata (name, logo, country)
-   **Requisition** - GoCardless OAuth flow state for account linking
-   **Agreement** - End-user agreement with institution for data access

### Data Flow

1. User initiates bank linking → Creates Requisition + Agreement via GoCardless
2. User completes OAuth → Requisition contains linked Account IDs
3. Background job syncs Transactions and Balances periodically
4. Frontend displays aggregated net worth from all Accounts

### GoCardless Integration

-   **Bank Account Data API** - PSD2-compliant open banking aggregation
-   Handles OAuth flow, token refresh, rate limiting automatically
-   Transactions fetched with configurable date range
-   Balances include booked, available, and pending amounts
