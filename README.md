# Networth Application

[![Backend CI](https://github.com/TomSB1423/NetWorth/actions/workflows/backend-ci.yml/badge.svg)](https://github.com/TomSB1423/NetWorth/actions/workflows/backend-ci.yml)
[![Frontend CI](https://github.com/TomSB1423/NetWorth/actions/workflows/frontend-ci.yml/badge.svg)](https://github.com/TomSB1423/NetWorth/actions/workflows/frontend-ci.yml)
[![Docs CI](https://github.com/TomSB1423/NetWorth/actions/workflows/docs-ci.yml/badge.svg)](https://github.com/TomSB1423/NetWorth/actions/workflows/docs-ci.yml)
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

**1. Create Local Configuration**

Copy the base configuration and create your local development overrides:

```shell
cp Networth.AppHost/appsettings.json Networth.AppHost/appsettings.Development.json
```

Edit `appsettings.Development.json` and fill in the required values:

- `postgres-password`: Any password for local PostgreSQL (e.g., `LocalDevPassword123!`)
- `firebase-api-key`: Your Firebase Web API key
- `firebase-auth-domain`: Your Firebase auth domain (e.g., `your-project.firebaseapp.com`)
- `firebase-project-id`: Your Firebase project ID

**2. GoCardless Secrets (Required for Bank Sync)**

GoCardless credentials are sensitive and must be stored in user secrets:

```shell
dotnet user-secrets set "Parameters:gocardless-secret-id" "YOUR_SECRET_ID" --project Networth.AppHost
dotnet user-secrets set "Parameters:gocardless-secret-key" "YOUR_SECRET_KEY" --project Networth.AppHost
```

### Standalone Functions Execution

If running the Azure Functions project strictly standalone (without Aspire), configure settings in `Networth.Functions/local.settings.json`.

## Database Migrations

Database migrations are managed explicitly and are **not** automatically applied at application startup.

### Local Development

For local development, apply migrations manually using the EF Core CLI:

```shell
dotnet ef database update --project Networth.Infrastructure
```

### Production Deployment

Migrations are automatically applied during CI/CD deployment (see `.github/workflows/deploy-dev.yml`). The deployment pipeline:

1. Provisions infrastructure via Terraform
2. Applies database migrations before deploying the application
3. Deploys the Functions API and Frontend

For manual migration against a deployed environment, use the migration script:

```shell
./infra/migrate.sh
```

This script retrieves connection details from Terraform outputs and Key Vault automatically.

## Testing

### System Tests (Playwright)

System tests use Playwright for end-to-end testing. You must install the Playwright browsers before running these tests:

Install the required browsers:

```shell
pwsh Tests/Networth.Functions.System.Tests/bin/Debug/net10.0/playwright.ps1 install
```

## License

This project is licensed under the GNU Affero General Public License v3.0 - see the [LICENSE](LICENSE) file for details.
