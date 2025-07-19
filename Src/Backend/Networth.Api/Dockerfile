# Simple multi-stage Dockerfile for .NET 9 API
# Production: docker build --target production -t networth-api:prod .
# Development: docker build --target development -t networth-api:dev .

# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files for dependency resolution
COPY ["src/Networth.Api/Networth.Api.csproj", "Networth.Api/"]
COPY ["src/Networth.Application/Networth.Application.csproj", "Networth.Application/"]
COPY ["src/Networth.Domain/Networth.Domain.csproj", "Networth.Domain/"]
COPY ["src/Networth.Infrastructure/Networth.Infrastructure.csproj", "Networth.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Networth.Api/Networth.Api.csproj"

# Copy source code and build
COPY src/ .
WORKDIR "/src/Networth.Api"
RUN dotnet build "Networth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Development stage - SDK image with debugging support
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS development
WORKDIR /src

# Install debugging tools
RUN apt-get update && apt-get install -y curl procps && rm -rf /var/lib/apt/lists/*

# Copy project files and restore
COPY ["src/Networth.Api/Networth.Api.csproj", "Networth.Api/"]
COPY ["src/Networth.Application/Networth.Application.csproj", "Networth.Application/"]
COPY ["src/Networth.Domain/Networth.Domain.csproj", "Networth.Domain/"]
COPY ["src/Networth.Infrastructure/Networth.Infrastructure.csproj", "Networth.Infrastructure/"]
RUN dotnet restore "Networth.Api/Networth.Api.csproj"

# Copy source code
COPY src/ .
WORKDIR "/src/Networth.Api"

# Configure development environment
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080
ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:8080"]

# Publish stage
FROM build AS publish
RUN dotnet publish "Networth.Api.csproj" -c Release -o /app/publish --no-restore

# Production stage
FROM base AS production

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app

USER appuser

# Copy published application
COPY --from=publish --chown=appuser:appuser /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Networth.Api.dll"]
