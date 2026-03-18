using Granit.Caching.StackExchangeRedis.Extensions;
using Granit.Core.Extensions;
using Granit.Diagnostics.Extensions;
using Granit.Http.ExceptionHandling.Extensions;
using Granit.Persistence.Extensions;
using Granit.Persistence.Hosting.Extensions;
using GranitMicroservice.CatalogService;
using Microsoft.EntityFrameworkCore;
using GranitMicroservice.CatalogService.Endpoints;
using GranitMicroservice.CatalogService.Persistence;
using GranitMicroservice.ServiceDefaults;
using GranitMicroservice.Shared.Hosting.Extensions;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ── Step 1 · Shared cross-cutting concerns ────────────────────────────────────
// AddSharedHostingAsync registers everything common to all services:
// observability (Serilog + OpenTelemetry), health checks, JWT bearer auth,
// Redis cache, and the Wolverine outbox backed by PostgreSQL.
// See SharedHostingModule for the full list of registered Granit modules.
await builder.AddSharedHostingAsync();

// ── Step 1b · Infrastructure health checks ───────────────────────────────────
// These checks drive /health/ready (readiness probe) and /health/startup.
// Aspire and Kubernetes gate traffic on these — the service is not considered
// ready until all three dependencies respond successfully.
//
// AddGranitDbContextHealthCheck — EF Core CanConnectAsync() through the same
//   DbContext configuration as production (connection pool, Npgsql options).
// AddGranitRedisHealthCheck — Redis PING via IConnectionMultiplexer already
//   registered by GranitCachingRedisModule; includes a 100ms degraded threshold.
// AddRabbitMqHealthCheck — opens a dedicated health-check connection from the
//   Aspire AMQP URI (amqp://user:pass@host:port/).
builder.Services.AddHealthChecks()
    .AddGranitDbContextHealthCheck<CatalogDbContext>()
    .AddGranitRedisHealthCheck();
builder.AddRabbitMqHealthCheck();

// ── Step 2 · Service-specific Granit modules ──────────────────────────────────
// AddGranitAsync wires the service's own domain modules (handlers, validators,
// domain services …). The module system deduplicates registrations so modules
// already loaded by SharedHostingAsync are not applied twice.
await builder.AddGranitAsync(granit => granit
    .AddModule<CatalogServiceModule>());

// ── Step 3 · EF Core DbContext ────────────────────────────────────────────────
// Each microservice owns its own isolated database — no shared DbContexts.
// AddGranitDbContext wraps AddDbContextFactory with ServiceLifetime.Scoped and
// wires all Granit interceptors automatically (AuditedEntityInterceptor,
// SoftDeleteInterceptor, …) via options.UseGranitInterceptors(sp).
// Scoped lifetime is required: interceptors depend on ICurrentUserService which
// is scoped — a singleton factory would try to resolve them from the root
// provider and throw at startup (notably during data seeding).
builder.Services.AddGranitDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("catalog-db")));

// ── Step 4 · OpenAPI ──────────────────────────────────────────────────────────
// Microsoft.AspNetCore.OpenApi generates the spec at /openapi/v1.json.
// The ApiGateway proxies this endpoint and surfaces it in a unified Scalar UI.
// Swashbuckle/NSwag are NOT used — ASP.NET Core's built-in generator is sufficient.
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// ── Step 5 · Granit middleware pipeline ───────────────────────────────────────
// UseGranitAsync runs module OnApplicationInitialization hooks (middleware
// registration, Wolverine startup, …). It does NOT apply EF Core migrations.
await app.UseGranitAsync();

// ── Step 5b · Database migrations (--migrate CLI flag) ───────────────────────
// Run `docker run myapp --migrate` (or `dotnet run -- --migrate`) in CI/CD or
// a K8s init container to apply pending EF Core migrations and seed data.
// In normal startup, this block is skipped entirely.
if (app.HasGranitMigrateFlag())
{
    await app.RunGranitMigrationsAsync();
    return;
}

// UseGranitExceptionHandling wraps the entire HTTP pipeline in a try/catch that
// maps exceptions to RFC 7807 Problem Details responses. Registered here, after
// UseGranitAsync, so Granit infrastructure is available during error handling
// (e.g., DI, correlation IDs). 5xx details are masked in non-Development
// environments (ISO 27001 compliance — no internal paths/SQL in responses).
app.UseGranitExceptionHandling();

// ── Step 6 · Endpoint mapping ─────────────────────────────────────────────────
// MapGranitHealthChecks → /health/live (always 200), /health/ready (readiness
//   tag), /health/startup (startup tag). Uses GranitHealthCheckWriter for
//   structured JSON output (Grafana/Loki compatible) and stampede-protected
//   caching (SemaphoreSlim double-check, 10s TTL).
// MapOpenApi            → /openapi/v1.json
// MapScalarApiReference → interactive API explorer at /scalar
// MapProductEndpoints   → domain HTTP endpoints (see Endpoints/ProductEndpoints.cs)
//
// Note: Granit module OnApplicationInitialization() only handles infrastructure
// bootstrapping. Route mapping is always explicit here to keep routing visible.
app.MapGranitHealthChecks();
app.MapOpenApi();
app.MapScalarApiReference();
app.MapProductEndpoints();

await app.RunAsync();
