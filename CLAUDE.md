# CLAUDE.md - Granit Microservice Template

## Project

- **Type**: .NET Aspire solution template for Granit microservices
- **Repo**: `granit-fx/granit-microservice-template` (GitHub, open-source)
- **License**: Apache-2.0
- **Framework**: Granit consumed via NuGet (`0.1.*`), NOT via ProjectReference
- **Reference repo**: `granit-dotnet` — see `templates/granit-api-full/` for patterns

## Stack and versions

.NET 10 (LTS) | C# 14 | .NET Aspire 9 | EF Core 10 | Wolverine 5.20+ | YARP

## Architecture

```text
src/
  GranitMicroservice.AppHost/          # .NET Aspire orchestration
  GranitMicroservice.ServiceDefaults/  # OpenTelemetry, health checks, resilience
  GranitMicroservice.ApiGateway/       # YARP reverse proxy + auth
  GranitMicroservice.Shared/           # Integration event contracts only
  GranitMicroservice.Shared.Hosting/   # Cross-cutting concerns (NO business packages)
  GranitMicroservice.IdentityService/
  GranitMicroservice.CatalogService/
  GranitMicroservice.NotificationService/

tests/
  GranitMicroservice.CatalogService.Tests/
  GranitMicroservice.CatalogService.Tests.Integration/
  GranitMicroservice.IdentityService.Tests/
  GranitMicroservice.NotificationService.Tests/

infra/keycloak-realms/                 # Keycloak realm JSON for Aspire import
```

## Commands

```bash
# Build entire solution
dotnet build GranitMicroservice.slnx

# Run tests
dotnet test GranitMicroservice.slnx

# Start all services via Aspire
dotnet run --project src/GranitMicroservice.AppHost

# Scaffold from template
dotnet new granit-microservice -n Acme.Platform -o /tmp/acme
dotnet build /tmp/acme/Acme.Platform.slnx
```

## Template mechanics

- `sourceName: "GranitMicroservice"` in `.template.config/template.json`
- `dotnet new` replaces all occurrences of `GranitMicroservice` with the user's name
- Solution file: `.slnx` (modern XML format, NOT `.sln`)
- Central Package Management: all versions in `Directory.Packages.props`

## Key conventions

### Shared.Hosting boundaries

Cross-cutting concerns ONLY. Allowed packages:

- `Granit.Core`, `Granit.Bundle.Essentials`
- `Granit.Wolverine.Postgresql`, `Granit.EventBus.Wolverine`
- `Granit.Authentication.JwtBearer`
- `Granit.Caching.StackExchangeRedis`
- `Granit.Http.Resilience`

FORBIDDEN in Shared.Hosting: `Granit.Identity`, `Granit.Notifications`,
`Granit.Authorization` — these belong in each service's `Program.cs` / module.

### Database isolation

Each microservice owns its PostgreSQL database. No shared databases.

### Communication patterns

1. **Synchrone**: Gateway to services via YARP; inter-service via HttpClient + resilience
2. **Asynchrone**: Wolverine + RabbitMQ + transactional outbox (PostgreSQL)
3. **Cache**: Redis via `Granit.Caching.StackExchangeRedis`

### No Docker Compose

Aspire orchestrates everything. No `docker-compose.yml`.

## Code quality

Follows `granit-dotnet` conventions (see its CLAUDE.md):

- C# 14: primary constructors, collection expressions, `field` keyword, extension members
- `[LoggerMessage]` for logging, `TimeProvider` for time, `[GeneratedRegex]` for regex
- `TypedResults` for API responses, `*Request`/`*Response` DTOs (never `*Dto`)
- File-scoped namespaces, `var` when type is apparent

## Anti-patterns

- No `DateTime.Now`/`UtcNow` — inject `TimeProvider`
- No `new Regex(...)` — use `[GeneratedRegex]`
- No Swashbuckle/NSwag — use `Microsoft.AspNetCore.OpenApi` + Scalar
- No `docker-compose` — Aspire only
- No shared DbContext across services
- No business packages in Shared.Hosting
