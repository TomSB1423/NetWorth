# Networth Application

[![Backend](https://github.com/TomSB1423/NetWorth/actions/workflows/backend.yml/badge.svg?branch=main)](https://github.com/TomSB1423/NetWorth/actions/workflows/backend.yml)
[![Frontend](https://github.com/TomSB1423/NetWorth/actions/workflows/frontend.yml/badge.svg?branch=main)](https://github.com/TomSB1423/NetWorth/actions/workflows/frontend.yml)
[![Docs](https://github.com/TomSB1423/NetWorth/actions/workflows/docs.yml/badge.svg?branch=main)](https://github.com/TomSB1423/NetWorth/actions/workflows/docs.yml)
[![Renovate](https://img.shields.io/badge/renovate-enabled-brightgreen.svg)](https://renovatebot.com)
[![Networth Docs](https://img.shields.io/badge/docs-Networth-blue)](https://networth.tbushell.co.uk/)

Networth is a personal financial aggregation application built with **.NET Aspire**. It allows users to track their net worth, view asset allocation, and sync accounts from various financial institutions using the GoCardless API.

[**Visit the Documentation Site**](https://networth.tbushell.co.uk/)

![Financial Dashboard](readme-assets/ui-dashboard.jpg)

## Features

- **Dashboard**: Real-time overview of Net Worth, Total Assets, Liabilities, and Cash Flow.
- **Asset Allocation**: Visual breakdown of assets (Cash, Crypto, Real Estate, Stocks).
- **Account Sync**: Integration with GoCardless to sync bank account balances and transactions.
- **Trend Analysis**: Historical view of net worth over time.

## Tech Stack

- **Orchestration**: .NET Aspire
- **Backend**: Azure Functions (.NET 10 Isolated Worker)
- **Frontend**: React (Vite + Tailwind CSS)
- **Database**: PostgreSQL (EF Core)
- **Infrastructure**: Azure Storage Queues (Azurite for local dev)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Node.js](https://nodejs.org/) (optional, for frontend development)

## Running with .NET Aspire

Run the complete application stack (PostgreSQL, Azure Functions, React frontend, Documentation):

```shell
dotnet run --project Networth.AppHost
```

This starts all services including:

- **PostgreSQL** with PgAdmin (port 5050)
- **Azure Functions** backend
- **React** frontend (port 3000)
- **Docusaurus** documentation (port 3001)
- **Aspire Dashboard** (<https://localhost:17065>)

## Documentation

Comprehensive architecture documentation is available in the Docusaurus site:

- **Local**: Run `dotnet run --project Networth.AppHost` and navigate to <http://localhost:3001>
- **Standalone**: See [Networth.Docs/README.md](Networth.Docs/README.md) for running docs independently

The documentation includes:

- Architecture overview and design patterns
- Component documentation (Functions, Application, Infrastructure, Domain)
- Data flow diagrams
- API reference with Scalar integration

## Configuration

### Local Development

Set the following user secrets in the AppHost project using the .NET CLI:

```shell
dotnet user-secrets set "Gocardless:SecretId" "YOUR_SECRET_ID" --project Networth.AppHost
dotnet user-secrets set "Gocardless:SecretKey" "YOUR_SECRET_KEY" --project Networth.AppHost
```

### Authentication (Microsoft Entra ID)

To enable authentication, you need to configure a Microsoft Entra ID (Azure AD) tenant with a Single Page Application (SPA) registration.

1. **Create an App Registration** in Azure Portal.
2. Add a **SPA** platform with Redirect URI: `http://localhost:3000`.
3. Configure **Google Federation** in External Identities if you want to use Google Sign-In.

Set the following secrets:

```shell
dotnet user-secrets set "entra-client-id" "YOUR_CLIENT_ID" --project Networth.AppHost
dotnet user-secrets set "entra-tenant-id" "YOUR_TENANT_ID" --project Networth.AppHost
```

The PostgreSQL password is configured in `appsettings.json`:

```json
"Parameters": {
    "postgres-password": "LocalDevPassword123!"
}
```

## Testing

### System Tests (Playwright)

System tests use Playwright for end-to-end testing. You must install the Playwright browsers before running these tests:

Install the required browsers:

```shell
pwsh Tests/Networth.Functions.System.Tests/bin/Debug/net10.0/playwright.ps1 install
```

## License

This project is licensed under the GNU Affero General Public License v3.0 - see the [LICENSE](LICENSE) file for details.
