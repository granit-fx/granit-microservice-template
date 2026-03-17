using Granit.Caching.StackExchangeRedis.Extensions;
using Granit.Core.Extensions;
using Granit.Diagnostics.Extensions;
using Granit.Http.ExceptionHandling.Extensions;
using Granit.Identity.Endpoints.Extensions;
using Granit.Identity.EntityFrameworkCore.Extensions;
using Granit.Identity.Keycloak.Extensions;
using Granit.Persistence.Extensions;
using GranitMicroservice.IdentityService;
using GranitMicroservice.IdentityService.Persistence;
using GranitMicroservice.ServiceDefaults;
using GranitMicroservice.Shared.Hosting.Extensions;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ── Step 1 · Shared cross-cutting concerns ────────────────────────────────────
// AddSharedHostingAsync registers observability, health checks, JWT bearer auth,
// Redis cache, and the Wolverine outbox. All services share this baseline.
await builder.AddSharedHostingAsync();

// ── Step 1b · Infrastructure health checks ───────────────────────────────────
// AddGranitKeycloakHealthCheck performs a client_credentials token request to
// verify Keycloak is reachable and the service-account client is valid.
// This is critical for the identity service: if Keycloak is down, user
// synchronization and token validation both fail.
builder.Services.AddHealthChecks()
    .AddGranitDbContextHealthCheck<IdentityServiceDbContext>()
    .AddGranitRedisHealthCheck()
    .AddGranitKeycloakHealthCheck();
builder.AddRabbitMqHealthCheck();

// ── Step 2 · Service-specific Granit modules ──────────────────────────────────
// IdentityServiceModule registers the Keycloak admin integration, user cache
// management, and related Wolverine handlers (e.g., UserSyncedHandler).
// It depends on GranitAuthenticationKeycloakModule, which is resolved
// automatically by the Granit module system via [DependsOn].
await builder.AddGranitAsync(granit => granit
    .AddModule<IdentityServiceModule>());

// ── Step 3 · EF Core DbContext ────────────────────────────────────────────────
// The identity service maintains a local read-model (user cache) synchronized
// from Keycloak via Wolverine messages. This allows fast JWT claim lookups
// without a round-trip to Keycloak on every authenticated request.
// AddGranitDbContext uses ServiceLifetime.Scoped so that Granit interceptors
// (AuditedEntityInterceptor, …) can be resolved from the scoped provider.
builder.Services.AddGranitDbContext<IdentityServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("identity-db")));

// ── Step 4 · Granit Identity EF Core store ───────────────────────────────────
// AddGranitIdentityEntityFrameworkCore wires the Granit.Identity persistence
// layer (user cache repository, EF migrations) to IdentityServiceDbContext.
// Must be called after AddDbContextFactory so the context type is already known.
builder.Services.AddGranitIdentityEntityFrameworkCore<IdentityServiceDbContext>();

// ── Step 5 · OpenAPI ──────────────────────────────────────────────────────────
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// ── Step 6 · Granit middleware pipeline ───────────────────────────────────────
// UseGranitAsync runs module OnApplicationInitialization hooks. It does NOT
// apply EF Core migrations.
await app.UseGranitAsync();

// ── Step 6b · EF Core migrations ─────────────────────────────────────────────
await using (var migrationScope = app.Services.CreateAsyncScope())
{
    var factory = migrationScope.ServiceProvider
        .GetRequiredService<IDbContextFactory<IdentityServiceDbContext>>();
    await using var db = await factory.CreateDbContextAsync();
    await db.Database.MigrateAsync();
}

// UseGranitExceptionHandling maps unhandled exceptions to RFC 7807 Problem
// Details responses. 5xx details masked in non-Development (ISO 27001).
app.UseGranitExceptionHandling();

// ── Step 7 · Endpoint mapping ─────────────────────────────────────────────────
// MapGranitHealthChecks → /health/live (always 200), /health/ready (readiness
//   tag), /health/startup (startup tag) with structured JSON and stampede cache.
// MapOpenApi            → /openapi/v1.json
// MapIdentityUserCacheEndpoints → REST endpoints for the local user cache
//   (GET /users, GET /users/{id}, …)
//   Granit modules register services but do NOT auto-map routes — always explicit.
// MapScalarApiReference → interactive API explorer at /scalar
app.MapGranitHealthChecks();
app.MapOpenApi();
app.MapIdentityUserCacheEndpoints();
app.MapScalarApiReference();

await app.RunAsync();
