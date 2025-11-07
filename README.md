# Networth Application

## Running with .NET Aspire

Run the complete application stack (PostgreSQL, Azure Functions, React frontend):

```shell
dotnet run --project Networth.AppHost
```

Access the Aspire Dashboard at: `https://localhost:17065`

Services:

-   **Azure Functions backend**: Managed by Aspire
-   **React frontend**: `http://localhost:3000`
-   **PostgreSQL database**: Containerized with PgAdmin
-   **Aspire Dashboard**: `https://localhost:17065`

## Configuration

Set the following user secrets using the .NET CLI:

```shell
dotnet user-secrets set "Gocardless:SecretId" "SECRET_VALUE"
dotnet user-secrets set "Gocardless:SecretKey" "SECRET_VALUE"
```
