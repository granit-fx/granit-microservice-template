# GranitMicroservice.Shared — Integration Event Contracts

This project contains **integration event contracts** shared across microservices.
It depends only on `Granit.Core` and must never contain business logic, services,
or infrastructure code.

## Rules

- All events are `sealed record` types implementing `IIntegrationEvent`
- Events contain only flat, serializable properties (no EF entities, no domain objects)
- Naming convention: `{Service}{Entity}{Action}Event` (e.g., `CatalogProductCreatedEvent`)

## Scaling trajectory

This shared C# project approach works well for:

- Small teams (1-5 developers)
- Monorepo setups
- Early-stage microservice architectures

For multi-team, large-scale setups, consider evolving to:

1. **Independent NuGet packages** — version contracts independently per service
2. **AsyncAPI / Protobuf** — generate DTOs from schema definitions to eliminate
   compile-time coupling between producer and consumer
3. **Schema registry** — centralized contract validation with backward compatibility
   checks (e.g., Confluent Schema Registry pattern)
