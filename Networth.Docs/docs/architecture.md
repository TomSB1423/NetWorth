# Architecture Quick Reference

This page provides a quick overview of the Networth application architecture. For detailed information, explore the specific architecture pages in the sidebar.

## System Architecture

The Networth application follows a **clean/onion architecture** pattern with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                    .NET Aspire                          │
│              (Orchestration Layer)                      │
└─────────────────────────────────────────────────────────┘
              │              │              │
    ┌─────────┴──────┐  ┌───┴────┐  ┌──────┴──────┐
    │   React SPA    │  │  Azure │  │  PostgreSQL │
    │   (Frontend)   │  │Functions│  │  Database   │
    └────────────────┘  └───┬────┘  └──────────────┘
                            │
              ┌─────────────┴──────────────┐
              │      Application Layer      │
              │  (Mediator, Handlers)       │
              └─────────────┬───────────────┘
                            │
              ┌─────────────┴──────────────┐
              │   Infrastructure Layer     │
              │ (EF Core, GoCardless API)  │
              └─────────────┬───────────────┘
                            │
              ┌─────────────┴──────────────┐
              │      Domain Layer          │
              │  (Entities, Interfaces)    │
              └────────────────────────────┘
```

## Key Components

### Networth.AppHost
.NET Aspire orchestrator that manages all services:
- PostgreSQL database with PgAdmin
- Azure Storage Emulator (Azurite) with queues
- Azure Functions backend
- React frontend
- Scalar API Reference

### Networth.Functions
Azure Functions backend with:
- HTTP triggers for API endpoints
- Queue triggers for background processing
- OpenAPI/Swagger documentation

### Networth.Application
Business logic layer using a custom mediator pattern:
- Commands and Queries
- Request Handlers
- FluentValidation validators

### Networth.Infrastructure
Data access and external integrations:
- EF Core with PostgreSQL
- GoCardless API integration (via Refit)
- Azure Queue Service

### Networth.Domain
Core domain entities and repository interfaces

### Networth.Frontend
React SPA with:
- Vite build tool
- Tailwind CSS styling
- Custom hooks for state management

## Data Flow

**Synchronous Flow**: 
React → Azure Functions (HTTP) → Mediator → Handler → Repository/External API → PostgreSQL/GoCardless

**Asynchronous Flow**: 
Functions enqueue to Azure Storage Queue → Queue trigger processes sync operations

## Next Steps

- [Architecture Overview](architecture/overview) - Detailed architecture documentation
- [.NET Aspire Setup](architecture/aspire) - Learn about the orchestration layer
- [Backend Architecture](architecture/backend) - Understand the Azure Functions backend
- [Data Flow](architecture/data-flow) - Explore how data flows through the system
