# Build stage - using .NET 10 SDK
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy solution and project files
COPY Directory.Build.props Directory.Packages.props ./
COPY Networth.Functions/Networth.Functions.csproj Networth.Functions/
COPY Networth.Application/Networth.Application.csproj Networth.Application/
COPY Networth.Infrastructure/Networth.Infrastructure.csproj Networth.Infrastructure/
COPY Networth.Domain/Networth.Domain.csproj Networth.Domain/
COPY Networth.ServiceDefaults/Networth.ServiceDefaults.csproj Networth.ServiceDefaults/

# Restore dependencies
RUN dotnet restore Networth.Functions/Networth.Functions.csproj

# Copy source code
COPY Networth.Functions/ Networth.Functions/
COPY Networth.Application/ Networth.Application/
COPY Networth.Infrastructure/ Networth.Infrastructure/
COPY Networth.Domain/ Networth.Domain/
COPY Networth.ServiceDefaults/ Networth.ServiceDefaults/

# Build and publish (skip analyzers for CI build)
WORKDIR /src/Networth.Functions
RUN dotnet publish -c Release -o /app/publish -p:RunAnalyzers=false -p:TreatWarningsAsErrors=false

# Runtime stage - Azure Functions v4 isolated worker with .NET 10
FROM --platform=linux/amd64 mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=build /app/publish /home/site/wwwroot
