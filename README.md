# Networth Application

## Running with .NET Aspire

Run the complete application stack (PostgreSQL, Azure Functions, React frontend):

```shell
dotnet run --project Networth.AppHost
```

Access the Aspire Dashboard at: `https://localhost:17065`

Services:

- **Azure Functions backend**: Managed by Aspire
- **API Documentation** (Scalar UI): Available at the Functions `/api/docs` endpoint
- **React frontend**: `http://localhost:3000`
- **PostgreSQL database**: Containerized with PgAdmin
- **Aspire Dashboard**: `https://localhost:17065`

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
