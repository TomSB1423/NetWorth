---
sidebar_position: 1
---

# Welcome to Networth Documentation

Welcome to the comprehensive documentation for the **Networth Application** - a personal financial aggregation platform built with modern .NET technologies.

## What is Networth?

Networth is a financial aggregation application that helps you track your net worth, view asset allocation, and sync accounts from various financial institutions using the GoCardless API. Built with .NET 9 and .NET Aspire, it provides a robust, scalable platform for personal financial management.

## Key Features

- 📊 **Real-time Dashboard**: Overview of Net Worth, Total Assets, Liabilities, and Cash Flow
- 💰 **Asset Allocation**: Visual breakdown of assets (Cash, Crypto, Real Estate, Stocks)
- 🔄 **Account Sync**: Integration with GoCardless to sync bank account balances and transactions
- 📈 **Trend Analysis**: Historical view of net worth over time

## Tech Stack

- **Orchestration**: .NET Aspire
- **Backend**: Azure Functions (.NET 9 Isolated Worker)
- **Frontend**: React (Vite + Tailwind CSS)
- **Database**: PostgreSQL (EF Core)
- **Infrastructure**: Azure Storage Queues (Azurite for local dev)

## Getting Started

To get started with Networth, check out the [Architecture Overview](architecture/overview) to understand the application structure, or jump to the [API Reference](/api) to explore the available endpoints.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Node.js](https://nodejs.org/) (optional, for frontend development)

## Quick Start

```bash
# Clone the repository
git clone https://github.com/TomSB1423/NetWorth.git
cd NetWorth

# Set up user secrets
dotnet user-secrets set "Gocardless:SecretId" "YOUR_SECRET_ID" --project Networth.AppHost
dotnet user-secrets set "Gocardless:SecretKey" "YOUR_SECRET_KEY" --project Networth.AppHost

# Run the application
dotnet run --project Networth.AppHost
```

The Aspire Dashboard will be available at https://localhost:17065

