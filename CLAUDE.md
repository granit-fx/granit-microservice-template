# CLAUDE.md - Granit Microservice Template

> Global conventions (git workflow, security, personas, DoD, refactoring, third-party
> licenses, markdown) live in `~/.claude/CLAUDE.md`. Code quality and anti-patterns
> follow `granit-dotnet/CLAUDE.md`. Neither is repeated here.

## Project

- **Type**: .NET Aspire solution template for Granit microservices
- **Repo**: `granit-fx/granit-microservice-template` (GitHub, open-source)
- **License**: Apache-2.0
- **Framework**: Granit consumed via NuGet (`0.1.*`), NOT via ProjectReference
- **Reference repo**: `granit-dotnet` — see `templates/granit-api-full/` for patterns

## Stack and versions

.NET 10 (LTS) | C# 14 | .NET Aspire 13 | EF Core 10 | Wolverine 6 | YARP

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
dotnet build GranitMicroservice.slnx
dotnet test GranitMicroservice.slnx
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

- `Granit`, `Granit.Bundle.Essentials`
- `Granit.Wolverine.Postgresql`, `Granit.EventBus.Wolverine`
- `Granit.Authentication.JwtBearer`
- `Granit.Caching.StackExchangeRedis`, `Granit.Caching.Vault`
- `Granit.Vault.HashiCorp` (Vault provider — auto-disabled in Development)
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
