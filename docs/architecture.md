# Architecture

## Design decisions

### Aspire over Docker Compose

.NET Aspire provides declarative C# orchestration with automatic service
discovery, connection string injection, and a built-in dashboard. No YAML
maintenance, no port conflicts, no manual environment variables.

### YARP over Ocelot

YARP is Microsoft's official reverse proxy, maintained alongside ASP.NET Core.
It supports config-as-code via `LoadFromMemory()`, integrates with Aspire
service discovery (`https+http://service-name`), and has no external
dependencies.

### Wolverine outbox over direct RabbitMQ

Wolverine's PostgreSQL transactional outbox guarantees at-least-once delivery
by writing events to the database within the same transaction as the domain
operation. If RabbitMQ is temporarily unavailable, events are delivered
automatically when it recovers.

### One database per service

Each microservice owns its PostgreSQL database. This enforces data isolation
and allows independent schema evolution. Cross-service data access happens
only through integration events or HTTP calls.

### Shared.Hosting boundaries

`Shared.Hosting` contains only cross-cutting infrastructure:

- Granit Essentials (timing, security, validation, persistence, observability)
- Wolverine + PostgreSQL outbox
- JWT Bearer authentication
- Redis caching
- HTTP resilience

Domain-specific modules (`Granit.Identity`, `Granit.Notifications`,
`Granit.Authorization`) stay in each service's own module class. This prevents
accidental coupling between services.

## Communication patterns

### Synchronous (HTTP)

```text
Client → API Gateway (YARP) → Service
```

- JWT validated at the gateway edge
- Aspire service discovery resolves addresses
- HTTP resilience (retry, circuit breaker, timeout) via `Granit.Http.Resilience`

### Asynchronous (Integration Events)

```text
CatalogService → [PostgreSQL outbox] → RabbitMQ → NotificationService
```

- Events implement `IIntegrationEvent` (defined in `Shared`)
- Published via `IDistributedEventBus` (Wolverine)
- Stored in PostgreSQL outbox before RabbitMQ delivery
- Handlers auto-discovered by Wolverine via convention

### Distributed cache

```text
Any Service → Redis (via Granit.Caching.StackExchangeRedis)
```

## Module dependency graph

```text
AppHost
  └── references all service projects (Aspire orchestration)

ApiGateway
  └── ServiceDefaults

IdentityService / CatalogService / NotificationService
  └── Shared.Hosting
        ├── ServiceDefaults (OpenTelemetry, health checks)
        └── Shared (integration event contracts)
```

## Health check probes

Each service exposes three Kubernetes-compatible probes:

| Probe | Path | Purpose |
|-------|------|---------|
| Liveness | `/health/live` | Is the process alive? |
| Readiness | `/health/ready` | Can it serve traffic? |
| Startup | `/health/startup` | Has it finished initializing? |

## Infrastructure persistence

All containers use `ContainerLifetime.Persistent` so data (PostgreSQL
rows, Redis cache, RabbitMQ queues) survives AppHost restarts during
development. Services use `WaitFor()` to avoid connection errors at startup.
