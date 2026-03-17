# Granit Microservice Template

A production-ready .NET Aspire solution template for building microservices
with the [Granit framework](https://github.com/granit-fx/granit-dotnet).

## What's included

| Project | Role |
|---------|------|
| **AppHost** | .NET Aspire orchestration (PostgreSQL, Redis, RabbitMQ, Keycloak) |
| **ServiceDefaults** | OpenTelemetry, health checks, resilient HTTP clients |
| **Shared** | Integration event contracts (`IIntegrationEvent`) |
| **Shared.Hosting** | Cross-cutting concerns (Wolverine outbox, JWT, Redis) |
| **CatalogService** | Domain entities, CRUD endpoints, event publishing |
| **IdentityService** | Keycloak integration via Granit.Identity |
| **NotificationService** | Event-driven Wolverine handlers |
| **ApiGateway** | YARP reverse proxy, JWT auth, rate limiting, Scalar UI |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/products/docker-desktop/) (for Aspire containers)
- Access to [Granit GitHub Packages](https://github.com/orgs/granit-fx/packages)

## Quick start

```bash
# Install the template
dotnet new install .

# Scaffold a new solution
dotnet new granit-microservice -n Acme.Platform -o ./acme-platform

# Run with Aspire
cd acme-platform
dotnet run --project src/Acme.Platform.AppHost
```

The Aspire dashboard opens automatically with all services, databases,
and infrastructure visible.

## Architecture

```text
┌─────────────┐
│  API Gateway │  YARP + JWT + Rate Limiting
│  (Scalar UI) │
└──────┬───┬──┘
       │   │
  ┌────┘   └────┐
  ▼              ▼
┌──────────┐  ┌──────────────┐
│ Identity │  │   Catalog    │
│ Service  │  │   Service    │──┐
└──────────┘  └──────────────┘  │ events
                                ▼
                        ┌───────────────┐
                        │ Notification  │
                        │   Service     │
                        └───────────────┘

Infrastructure (Aspire-managed):
  PostgreSQL (1 DB per service) │ Redis │ RabbitMQ │ Keycloak
```

## Communication patterns

1. **Synchronous**: Gateway to services via YARP, inter-service via
   HttpClient + resilience + Aspire service discovery
2. **Asynchronous**: Wolverine + RabbitMQ + PostgreSQL transactional outbox
3. **Distributed cache**: Redis via `Granit.Caching.StackExchangeRedis`

## Documentation

- [Getting started](docs/getting-started.md) — step-by-step setup guide
- [Architecture](docs/architecture.md) — design decisions and patterns

## License

[Apache-2.0](LICENSE)
