#!/bin/bash

# Script to run EF Core migrations for NetWorth application
# This script extracts connection details from the running Aspire PostgreSQL container
# and applies pending migrations to the database.

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}🔄 NetWorth Database Migration Script${NC}"
echo ""

# Check if Docker is running
if ! docker ps >/dev/null 2>&1; then
    echo -e "${RED}❌ Error: Docker is not running. Please start Docker and try again.${NC}"
    exit 1
fi

# Find the PostgreSQL container
POSTGRES_CONTAINER=$(docker ps --format "{{.Names}}" | grep -E "postgres-[a-z0-9]+$" | head -n 1)

if [ -z "$POSTGRES_CONTAINER" ]; then
    echo -e "${RED}❌ Error: PostgreSQL container not found.${NC}"
    echo -e "${YELLOW}💡 Tip: Make sure Aspire is running by executing:${NC}"
    echo -e "   dotnet run --project Networth.AppHost"
    exit 1
fi

echo -e "${GREEN}✅ Found PostgreSQL container: ${POSTGRES_CONTAINER}${NC}"

# Extract port
POSTGRES_PORT=$(docker ps --format "{{.Names}} {{.Ports}}" | grep "$POSTGRES_CONTAINER" | sed -n 's/.*127.0.0.1:\([0-9]*\)->5432\/tcp.*/\1/p')

if [ -z "$POSTGRES_PORT" ]; then
    echo -e "${RED}❌ Error: Could not extract PostgreSQL port.${NC}"
    exit 1
fi

echo -e "${GREEN}✅ PostgreSQL port: ${POSTGRES_PORT}${NC}"

# Extract password
POSTGRES_PASSWORD=$(docker exec "$POSTGRES_CONTAINER" env | grep "^POSTGRES_PASSWORD=" | cut -d '=' -f2)

if [ -z "$POSTGRES_PASSWORD" ]; then
    echo -e "${RED}❌ Error: Could not extract PostgreSQL password.${NC}"
    exit 1
fi

echo -e "${GREEN}✅ PostgreSQL credentials extracted${NC}"
echo ""

# Build connection string
CONNECTION_STRING="Host=localhost;Port=${POSTGRES_PORT};Database=networth-db;Username=postgres;Password=${POSTGRES_PASSWORD}"

# Navigate to Infrastructure project
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="${SCRIPT_DIR}/../Networth.Infrastructure"

if [ ! -d "$PROJECT_DIR" ]; then
    echo -e "${RED}❌ Error: Infrastructure project directory not found at ${PROJECT_DIR}${NC}"
    exit 1
fi

cd "$PROJECT_DIR"

# Check if a migration name was provided for creating new migration
if [ "$1" == "add" ]; then
    if [ -z "$2" ]; then
        echo -e "${RED}❌ Error: Migration name required when using 'add' command${NC}"
        echo -e "${YELLOW}Usage: $0 add <MigrationName>${NC}"
        exit 1
    fi

    echo -e "${YELLOW}📝 Creating new migration: $2${NC}"
    env "ConnectionStrings__networth-db=${CONNECTION_STRING}" \
        dotnet ef migrations add "$2" --output-dir Data/Migrations

    echo -e "${GREEN}✅ Migration created successfully!${NC}"
    echo ""
fi

# Apply migrations
echo -e "${YELLOW}🚀 Applying pending migrations...${NC}"
env "ConnectionStrings__networth-db=${CONNECTION_STRING}" \
    dotnet ef database update

echo ""
echo -e "${GREEN}✅ Migration completed successfully!${NC}"
echo ""
echo -e "${YELLOW}Connection details:${NC}"
echo -e "  Host: localhost"
echo -e "  Port: ${POSTGRES_PORT}"
echo -e "  Database: networth-db"
echo -e "  User: postgres"
