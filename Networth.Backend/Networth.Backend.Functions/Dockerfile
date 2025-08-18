# Build and publish Azure Functions (.NET 9 isolated)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and restore first to leverage Docker layer caching
COPY Networth.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY global.json ./

# Copy project files
COPY Networth.Backend/Networth.Backend.Functions/Networth.Backend.Functions.csproj Networth.Backend/Networth.Backend.Functions/
COPY Networth.Backend/Networth.Backend.Infrastructure/Networth.Backend.Infrastructure.csproj Networth.Backend/Networth.Backend.Infrastructure/
COPY Networth.Backend/Networth.Backend.Application/Networth.Backend.Application.csproj Networth.Backend/Networth.Backend.Application/
COPY Networth.Backend/Networth.Backend.Domain/Networth.Backend.Domain.csproj Networth.Backend/Networth.Backend.Domain/

RUN dotnet restore Networth.Backend/Networth.Backend.Functions/Networth.Backend.Functions.csproj --nologo

# Copy the rest of the source and publish
COPY Networth.Backend/ Networth.Backend/
RUN dotnet publish Networth.Backend/Networth.Backend.Functions/Networth.Backend.Functions.csproj -c Release -o /app/publish --no-restore --nologo
 # Ensure settings.json is available at runtime (it's excluded from publish by csproj)
COPY Networth.Backend/Networth.Backend.Functions/settings.json /app/publish/settings.json

# Runtime image for Azure Functions .NET isolated
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated9.0

# Azure Functions container listens on 80 by default
EXPOSE 80

WORKDIR /home/site/wwwroot
COPY --from=build /app/publish .

# Ensure a writable data mount point for SQLite (compose will mount /data)
VOLUME ["/data"]
