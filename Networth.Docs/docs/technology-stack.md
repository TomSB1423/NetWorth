---
sidebar_label: Technology Stack
sidebar_position: 3
---

# Technology Stack

Networth is built using modern, reliable technologies to ensure performance, security, and scalability. Here is a high-level view of the resources that power the application.

## System Overview

The application is composed of a frontend dashboard, a backend API, and secure data storage, all orchestrated together.

```mermaid
graph TB
    %% C4 Style Classes
    classDef person fill:#2c3e50,stroke:#1a252f,color:#fff,stroke-width:2px;
    classDef container fill:#8226d6,stroke:#5b1a96,color:#fff,stroke-width:2px;
    classDef component fill:#a866e2,stroke:#7522c1,color:#fff,stroke-width:2px;
    classDef database fill:#336791,stroke:#1a3a52,color:#fff,stroke-width:2px;
    classDef external fill:#95a5a6,stroke:#7f8c8d,color:#fff,stroke-width:2px;

    %% Actors
    User((User)):::person

    %% External Systems
    GoCardless[GoCardless API<br/>External Banking Provider]:::external

    %% Networth System Boundary
    subgraph "Networth System"
        direction TB

        SPA[SPA<br/>React, Vite]:::container

        subgraph "Azure Functions App"
            direction TB
            HttpFunc[HTTP Triggers<br/>API Controllers]:::component
            QueueFunc[Queue Triggers<br/>Background Workers]:::component
        end

        Postgres[(PostgreSQL<br/>Database)]:::database
        Storage[[Azure Storage<br/>Queues]]:::database
    end

    %% Relationships
    User -->|Uses| SPA
    SPA -->|JSON/HTTPS| HttpFunc

    HttpFunc -->|Reads/Writes| Postgres
    HttpFunc -->|Enqueues| Storage

    QueueFunc -->|Dequeues| Storage
    QueueFunc -->|Writes| Postgres
    QueueFunc -->|Syncs Data| GoCardless
```

## Core Technologies

<div className="row">
  <div className="col col--4">
    <div className="card margin-bottom--md">
      <div className="card__header">
        <h3>🎨 Frontend</h3>
      </div>
      <div className="card__body">
        <ul>
          <li><strong>React</strong>: Component-based UI library</li>
          <li><strong>Vite</strong>: Next-generation frontend tooling</li>
          <li><strong>Tailwind CSS</strong>: Utility-first CSS framework</li>
          <li><strong>Recharts</strong>: Composable charting library</li>
        </ul>
      </div>
    </div>
  </div>

  <div className="col col--4">
    <div className="card margin-bottom--md">
      <div className="card__header">
        <h3>⚙️ Backend</h3>
      </div>
      <div className="card__body">
        <ul>
          <li><strong>.NET 9</strong>: Latest high-performance framework</li>
          <li><strong>Azure Functions</strong>: Serverless compute</li>
          <li><strong>.NET Aspire</strong>: Cloud-native orchestration</li>
          <li><strong>Refit</strong>: Type-safe REST library</li>
        </ul>
      </div>
    </div>
  </div>

  <div className="col col--4">
    <div className="card margin-bottom--md">
      <div className="card__header">
        <h3>💾 Data & Infra</h3>
      </div>
      <div className="card__body">
        <ul>
          <li><strong>PostgreSQL</strong>: Relational database</li>
          <li><strong>Entity Framework Core</strong>: ORM</li>
          <li><strong>Azurite</strong>: Local Azure Storage emulator</li>
          <li><strong>Docker</strong>: Containerization</li>
        </ul>
      </div>
    </div>
  </div>
</div>

## Architecture Highlights

### 1. .NET Aspire Orchestration

Networth uses **.NET Aspire** to manage the local development environment. It automatically spins up the React frontend, Azure Functions backend, PostgreSQL database, and Azurite emulator, handling service discovery and connection strings automatically.

### 2. Serverless Backend

The backend is built entirely on **Azure Functions** (Isolated Worker model). This ensures the application scales to zero when not in use (saving costs) and scales up instantly when needed.

### 3. Clean Architecture

The solution follows a strict **Clean Architecture** pattern:

- **Domain**: Core entities and interfaces (No dependencies)
- **Application**: Business logic, commands, and queries
- **Infrastructure**: Database implementation and external API clients
- **Functions**: Entry points (Controllers)

### 4. Secure Aggregation

We use **GoCardless** (formerly Nordigen) for Open Banking connectivity. Networth never stores your bank credentials; it only stores the secure tokens needed to fetch read-only transaction data.
