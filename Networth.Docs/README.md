# Networth.Docs - Documentation Site

This directory contains the Docusaurus-based documentation site for the Networth application, built using [Docusaurus](https://docusaurus.io/).

## Overview

The documentation site provides comprehensive architecture documentation and API reference using:
- **Docusaurus v3** - Documentation framework
- **Scalar** - Interactive API documentation with OpenAPI integration

## Running Locally

### Standalone Mode (Documentation Only)

```bash
cd Networth.Docs
npm install
npm start
```

The site will be available at http://localhost:3000

**Note**: The API Reference page will be empty when running standalone because it requires the Functions service to provide the OpenAPI spec.

### Full Application Mode (Recommended)

Run the entire application stack with Aspire:

```bash
# From repository root
dotnet run --project Networth.AppHost
```

This starts:
- Documentation site on port 3001
- Functions service (provides OpenAPI spec)
- All other services

The Aspire Dashboard will show all running services at https://localhost:17065

## Features

### Architecture Documentation

Comprehensive documentation covering:
- **Architecture Overview** - System architecture and design patterns
- **.NET Aspire Orchestration** - Service management and configuration
- **Backend Architecture** - Azure Functions and API layer
- **Frontend Architecture** - React SPA structure
- **Data Flow** - Request/response flows and data management
- **Component Documentation** - Detailed component guides

### API Reference (Scalar Integration)

The `/api` route provides interactive API documentation powered by Scalar:
- Auto-generated from OpenAPI spec
- Try-it-out functionality
- Request/response examples
- Purple theme matching the brand

The API spec is pulled from the Functions service at runtime.

## Build

```bash
npm run build
```

This command generates static content into the `build` directory and can be served using any static contents hosting service.

## Development

### Adding New Documentation

1. Create `.md` files in the `docs/` directory
2. Update `sidebars.ts` to include new pages
3. Follow the existing structure and formatting

### Configuration

Main configuration in `docusaurus.config.ts`:
- Site metadata
- Navigation structure
- Scalar plugin settings
- Theme configuration

## Dependencies

Key packages:
- `@docusaurus/core` - Core framework
- `@docusaurus/preset-classic` - Standard Docusaurus features
- `@scalar/docusaurus` - API reference integration

## Environment Variables

When running with Aspire, the following are set automatically:
- `PORT` - HTTP port (default: 3001)
- `API_SPEC_URL` - OpenAPI spec location
- `BROWSER` - Set to "none" to prevent auto-opening browser

