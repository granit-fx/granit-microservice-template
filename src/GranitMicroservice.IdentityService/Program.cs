using Granit.Auditing.EntityFrameworkCore.Extensions;
using Granit.Caching.StackExchangeRedis.Extensions;
using Granit.Extensions;
using Granit.Diagnostics.Extensions;
using Granit.Http.ApiDocumentation.Extensions;
using Granit.Http.Cors.Extensions;
using Granit.Http.ExceptionHandling.Extensions;
using Granit.Http.SecurityHeaders.Extensions;
using Granit.Identity.Endpoints.Extensions;
using Granit.Identity.EntityFrameworkCore.Extensions;
using Granit.Identity.Federated.EntityFrameworkCore.Extensions;
using Granit.Identity.Federated.Keycloak.Extensions;
using Granit.Persistence.EntityFrameworkCore.Extensions;
using Granit.Persistence.EntityFrameworkCore.Hosting.Extensions;
using Granit.Persistence.MultiTenancy;
using GranitMicroservice.IdentityService;
using Microsoft.EntityFrameworkCore;
using GranitMicroservice.IdentityService.Persistence;
using GranitMicroservice.ServiceDefaults;
using GranitMicroservice.Shared.Hosting.Extensions;

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
// It depends on GranitAuthenticationJwtBearerKeycloakModule, which is resolved
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
// Two registrations, both required:
//   1. Federated cache (Granit.Identity.Federated.EntityFrameworkCore) — replaces
//      NullUserLookupService with CachedUserLookupService and wires the federated
//      identity cache against its own IdentityFederatedHostDbContext. Since the
//      Granit DualScopeStorageMode V2 API (Epic #2382) this is registered on the
//      IHostApplicationBuilder via an options object: StorageMode selects the
//      physical layout, Configure supplies the EF Core provider + connection.
//   2. Canonical user directory (Granit.Identity.EntityFrameworkCore) — registers
//      the IdentityHostDbContext + IUserDirectoryWriter required by
//      CachedUserLookupService to materialise the User aggregate on cache miss.
//      Migrations for both tables live on IdentityServiceDbContext (see
//      ConfigureIdentityModule + ConfigureGranitIdentityModule in OnGranitModelCreating).
// Both adopt the Granit DualScopeStorageMode V2 API: registered on the
// IHostApplicationBuilder via an options object whose StorageMode selects the
// physical layout and whose Configure supplies the EF Core provider + connection.
builder.AddGranitIdentityFederatedEntityFrameworkCore(opts =>
{
    // Shared (default): a single host table holds every federated identity; tenant
    // rows carry a TenantId filtered by a row-level query filter. Behavioural
    // equivalent of the pre-V2 registration.
    opts.StorageMode = DualScopeStorageMode.Shared;
    opts.Configure = db => db.UseNpgsql(builder.Configuration.GetConnectionString("identity-db"));

    // Pour activer l'isolation physique par tenant (RGPD Art. 17, ISO 27001 A.8.12):
    // opts.StorageMode = DualScopeStorageMode.Segregated;
    // opts.ConfigureHost = db => db.UseNpgsql(hostConnString);
    // opts.ConfigureSchemaPerTenant = db => db.UseNpgsql(baseConnString);
});
builder.AddGranitIdentityEntityFrameworkCore(opts =>
{
    opts.StorageMode = DualScopeStorageMode.Shared;
    opts.Configure = db => db.UseNpgsql(builder.Configuration.GetConnectionString("identity-db"));

    // Pour activer l'isolation physique par tenant (RGPD Art. 17, ISO 27001 A.8.12):
    // opts.StorageMode = DualScopeStorageMode.Segregated;
    // opts.ConfigureHost = db => db.UseNpgsql(hostConnString);
    // opts.ConfigureSchemaPerTenant = db => db.UseNpgsql(baseConnString);
});

// ── Step 4b · Audit trail EF Core store ───────────────────────────────────────
// IdentityServiceModule pulls GranitAuditingModule transitively (via
// GranitIdentityModule), which runs an AuditingCleanupWorker and emits an audit
// trail for identity operations. AddGranitAuditingEntityFrameworkCore swaps the
// default no-op stores for durable EF Core ones (writer/reader/cleaner). Shared
// mode keeps the trail in the identity database; migrations for the audit tables
// are owned by IdentityServiceDbContext via ConfigureAuditingModule.
builder.AddGranitAuditingEntityFrameworkCore(opts =>
{
    opts.StorageMode = DualScopeStorageMode.Shared;
    opts.Configure = db => db.UseNpgsql(builder.Configuration.GetConnectionString("identity-db"));

    // Pour activer l'isolation physique par tenant (RGPD Art. 17, ISO 27001 A.8.12):
    // opts.StorageMode = DualScopeStorageMode.Segregated;
    // opts.ConfigureHost = db => db.UseNpgsql(hostConnString);
    // opts.ConfigureSchemaPerTenant = db => db.UseNpgsql(baseConnString);
});

// ── Step 5 · CORS ─────────────────────────────────────────────────────────────
// Strict BFF in production: the SPA only talks to the gateway, never to this
// backend cross-origin, so Http:Cors:AllowedOrigins is empty in appsettings.json
// and the default policy denies every cross-origin request. The Development
// override (appsettings.Development.json) opens the gateway origin so Scalar's
// "Try it" button can call backend endpoints from the gateway-hosted Scalar UI.
builder.AddGranitCors();

WebApplication app = builder.Build();

// ── Step 6 · Granit middleware pipeline ───────────────────────────────────────
// UseGranitAsync runs module OnApplicationInitialization hooks. It does NOT
// apply EF Core migrations.
await app.UseGranitAsync();

// ── Step 6b · Database migrations (--migrate CLI flag) ───────────────────────
// Run `docker run myapp --migrate` in CI/CD or a K8s init container.
// In normal startup, this block is skipped entirely.
if (app.HasGranitMigrateFlag())
{
    await app.RunGranitMigrationsAsync();
    return;
}

// UseGranitExceptionHandling maps unhandled exceptions to RFC 7807 Problem
// Details responses. 5xx details masked in non-Development (ISO 27001).
app.UseGranitExceptionHandling();
app.UseCors();
app.UseGranitSecurityHeaders();

// ── Step 6 · Endpoint mapping ─────────────────────────────────────────────────
// MapGranitHealthChecks      → /health/live, /health/ready, /health/startup
// MapGranitIdentityUserCache → REST endpoints for the local user cache
//   (GET /users, GET /users/{id}, …). Granit modules register services but do
//   NOT auto-map routes — routing stays explicit here.
// UseGranitApiDocumentation  → /openapi/v{N}.json + /scalar interactive UI.
//   With ApiDocumentation:OAuth2 configured (see appsettings.json), Scalar's
//   "Authorize" button runs an OAuth2 Authorization Code + PKCE flow against
//   Keycloak instead of asking for a raw bearer token.
app.MapGranitHealthChecks();
app.MapGranitIdentityUserCache();
app.UseGranitApiDocumentation();

await app.RunAsync();
