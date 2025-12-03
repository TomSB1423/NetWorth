# Networth Application

![Financial Dashboard](docs/dashboard.png)

## Overview

Networth is a personal financial aggregation application built with **.NET 9** and **.NET Aspire**. It allows users to track their net worth, view asset allocation, and sync accounts from various financial institutions using the GoCardless API.

## Features

- **Dashboard**: Real-time overview of Net Worth, Total Assets, Liabilities, and Cash Flow.
- **Asset Allocation**: Visual breakdown of assets (Cash, Crypto, Real Estate, Stocks).
- **Account Sync**: Integration with GoCardless to sync bank account balances and transactions.
- **Trend Analysis**: Historical view of net worth over time.

## Tech Stack

- **Orchestration**: .NET Aspire
- **Backend**: Azure Functions (.NET 9 Isolated Worker)
- **Frontend**: React (Vite + Tailwind CSS)
- **Database**: PostgreSQL (EF Core)
- **Infrastructure**: Azure Storage Queues (Azurite for local dev)

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Node.js](https://nodejs.org/) (optional, for frontend development)

## Running with .NET Aspire

Run the complete application stack (PostgreSQL, Azure Functions, React frontend):

```shell
dotnet run --project Networth.AppHost
```

## Configuration

### Local Development

Set the following user secrets in the AppHost project using the .NET CLI:

```shell
dotnet user-secrets set "Gocardless:SecretId" "YOUR_SECRET_ID" --project Networth.AppHost
dotnet user-secrets set "Gocardless:SecretKey" "YOUR_SECRET_KEY" --project Networth.AppHost
```

The PostgreSQL password is configured in `appsettings.json`:

```json
"Parameters": {
    "postgres-password": "LocalDevPassword123!"
}
```

## License

This project is licensed under the GNU Affero General Public License v3.0 - see the [LICENSE](LICENSE) file for details.
