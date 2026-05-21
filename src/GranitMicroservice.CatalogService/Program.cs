using Granit.Caching.StackExchangeRedis.Extensions;
using Granit.Extensions;
using Granit.Diagnostics.Extensions;
using Granit.Http.ApiDocumentation.Extensions;
using Granit.Http.Cors.Extensions;
using Granit.Http.ExceptionHandling.Extensions;
using Granit.Http.SecurityHeaders.Extensions;
using Granit.Persistence.EntityFrameworkCore.Extensions;
using Granit.Persistence.EntityFrameworkCore.Hosting.Extensions;
using GranitMicroservice.CatalogService;
using Microsoft.EntityFrameworkCore;
using GranitMicroservice.CatalogService.Endpoints;
using GranitMicroservice.CatalogService.Persistence;
using GranitMicroservice.ServiceDefaults;
using GranitMicroservice.Shared.Hosting.Extensions;

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
//   registered by GranitCachingStackExchangeRedisModule; includes a 100ms degraded threshold.
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

// ── Step 4 · CORS ─────────────────────────────────────────────────────────────
// Strict BFF in production: the SPA only talks to the gateway, never to this
// backend cross-origin, so Http:Cors:AllowedOrigins is empty in appsettings.json
// and the default policy denies every cross-origin request. The Development
// override (appsettings.Development.json) opens the gateway origin so Scalar's
// "Try it" button can call backend endpoints from the gateway-hosted Scalar UI.
builder.AddGranitCors();

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
app.UseCors();
app.UseGranitSecurityHeaders();

// ── Step 5 · Endpoint mapping ─────────────────────────────────────────────────
// MapGranitHealthChecks      → /health/live, /health/ready, /health/startup
// UseGranitApiDocumentation  → /openapi/v{N}.json + /scalar interactive UI.
//   When ApiDocumentation:OAuth2 is configured (see appsettings.json), the
//   Bearer security scheme is replaced by an OAuth2 Authorization Code + PKCE
//   flow against Keycloak, so the "Authorize" button in Scalar performs a real
//   login round-trip instead of asking for a raw bearer token.
// MapProductEndpoints        → domain HTTP endpoints (see Endpoints/ProductEndpoints.cs)
app.MapGranitHealthChecks();
app.UseGranitApiDocumentation();
app.MapProductEndpoints();

await app.RunAsync();
