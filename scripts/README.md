# Database Migration Scripts

This directory contains scripts to help manage database migrations for the NetWorth application.

## run-migrations.sh

A bash script that automates the process of running EF Core migrations against the PostgreSQL database running in Aspire.

### Prerequisites

- Docker must be running
- Aspire must be running (via `dotnet run --project Networth.AppHost`)
- .NET EF Core tools installed globally (if not: `dotnet tool install --global dotnet-ef`)

### Usage

#### Apply Pending Migrations

From the repository root:

```bash
./scripts/run-migrations.sh
```

This will:

1. Detect the running PostgreSQL container from Aspire
2. Extract connection credentials (port and password)
3. Apply all pending migrations to the database

#### Create a New Migration

```bash
./scripts/run-migrations.sh add <MigrationName>
```

Example:

```bash
./scripts/run-migrations.sh add AddUserPreferences
```

This will:

1. Create a new migration file in `Networth.Infrastructure/Data/Migrations/`
2. Automatically apply it to the running database

### What the Script Does

1. **Checks Docker** - Verifies Docker is running
2. **Finds PostgreSQL Container** - Searches for the Aspire-managed postgres container
3. **Extracts Credentials** - Gets the dynamic port and password from the container
4. **Builds Connection String** - Creates the connection string for EF Core
5. **Runs Migrations** - Executes `dotnet ef` commands with the proper connection string

### Troubleshooting

**Error: PostgreSQL container not found**

- Make sure Aspire is running: `dotnet run --project Networth.AppHost`
- Wait a few seconds for containers to start

**Error: Docker is not running**

- Start Docker Desktop and try again

**Error: Infrastructure project directory not found**

- Make sure you're running the script from the repository root

### Manual Migration (Alternative)

If you prefer to run migrations manually:

```bash
# 1. Get PostgreSQL container name
docker ps | grep postgres

# 2. Get credentials
docker exec <container-name> env | grep POSTGRES

# 3. Run migration
cd Networth.Infrastructure
env "ConnectionStrings__networth-db=Host=localhost;Port=<PORT>;Database=networth-db;Username=postgres;Password=<PASSWORD>" \
  dotnet ef database update
```

See `.github/copilot-instructions.md` for more details on the manual process.
