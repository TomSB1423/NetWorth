# Networth Application

## Instructions

Set the following user secrets using the .NET CLI:

```shell
dotnet user-secrets set "Gocardless:SecretId" "SECRET_VALUE"
dotnet user-secrets set "Gocardless:SecretKey" "SECRET_VALUE"
```

### Running the backend with Docker

The Docker Compose file was moved to `Networth.Backend/docker-compose.yml` and only includes the backend and Azurite (no frontend).

From the repository root:

```shell
cd Networth.Backend
docker compose up --build
```

Services:

-   Backend (Azure Functions): [http://localhost:7071](http://localhost:7071)
-   Azurite (Storage emulator): Blob 10000, Queue 10001, Table 10002
