# Getting started

## Prerequisites

1. **.NET 10 SDK** — [download](https://dotnet.microsoft.com/download/dotnet/10.0)
2. **Docker Desktop** — required for Aspire containers
3. **GitLab Package Registry access** — credentials for the Granit.* NuGet feed
   on `gitlab.digitaldynamics.be`

### Configure NuGet credentials

The `nuget.config` already declares the `GranitGitLab` package source. Provide
credentials via the environment variable below (matches the source key,
avoids writing the token to disk):

```bash
export NuGetPackageSourceCredentials_GranitGitLab="Username=$GITLAB_USER;Password=$GITLAB_DEPLOY_TOKEN"
```

The deploy token needs the `read_package_registry` scope.

## Scaffold a new solution

```bash
# Install the template (from local clone or NuGet)
dotnet new install .

# Create your solution
dotnet new granit-microservice -n Acme.Platform -o ./acme-platform
cd acme-platform
```

All files are renamed: `GranitMicroservice` becomes `Acme.Platform`.

## Build and run

```bash
# Restore and build
dotnet build Acme.Platform.slnx

# Start everything via Aspire
dotnet run --project src/Acme.Platform.AppHost
```

The Aspire dashboard opens at `https://localhost:17178`.

## What starts automatically

| Resource | Port | Notes |
|----------|------|-------|
| PostgreSQL | 5432 | 3 databases (identity, catalog, notification) |
| PgAdmin | auto | PostgreSQL management UI |
| Redis | 6379 | Distributed cache |
| Redis Insight | auto | Redis management UI |
| RabbitMQ | 5672 | Message broker |
| RabbitMQ Management | 15672 | Queue management UI |
| Keycloak | 8080 | Pre-configured realm with test users |
| Identity Service | auto | User management |
| Catalog Service | auto | Product CRUD + events |
| Notification Service | auto | Event-driven notifications |
| API Gateway | auto | Single entry point |

All containers use `ContainerLifetime.Persistent` — data survives
AppHost restarts.

## Test users (Keycloak)

| Username | Password | Roles |
|----------|----------|-------|
| `admin` | `admin` | admin, user |
| `testuser` | `test` | user |

Keycloak admin console: `http://localhost:8080` (admin/admin).

## Run tests

```bash
# Unit tests
dotnet test Acme.Platform.slnx --filter "FullyQualifiedName!~Integration"

# Integration tests (requires Docker)
dotnet test Acme.Platform.slnx --filter "FullyQualifiedName~Integration"
```

## Project structure

```text
acme-platform/
├── src/
│   ├── Acme.Platform.AppHost/            # Aspire orchestration
│   ├── Acme.Platform.ServiceDefaults/    # Shared infra (OTel, health)
│   ├── Acme.Platform.Shared/             # Integration event contracts
│   ├── Acme.Platform.Shared.Hosting/     # Cross-cutting Granit config
│   ├── Acme.Platform.ApiGateway/         # YARP reverse proxy
│   ├── Acme.Platform.IdentityService/    # Keycloak + Granit.Identity
│   ├── Acme.Platform.CatalogService/     # Domain + endpoints + events
│   └── Acme.Platform.NotificationService/ # Event-driven handlers
├── tests/
│   ├── Acme.Platform.CatalogService.Tests/
│   ├── Acme.Platform.CatalogService.Tests.Integration/
│   ├── Acme.Platform.IdentityService.Tests/
│   └── Acme.Platform.NotificationService.Tests/
└── infra/keycloak-realms/                 # Keycloak realm config
```

## Next steps

- Add domain entities to CatalogService
- Add Wolverine handlers for new integration events
- Configure production Keycloak (replace dev secrets)
- Add EF Core migrations: `dotnet ef migrations add Init -p src/Acme.Platform.CatalogService`
