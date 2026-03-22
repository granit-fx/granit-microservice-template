using Granit.Caching.StackExchangeRedis.Extensions;
using Granit.Core.Extensions;
using Granit.Diagnostics.Extensions;
using Granit.Http.ExceptionHandling.Extensions;
using Granit.Persistence.Extensions;
using Granit.Persistence.Hosting.Extensions;
using GranitMicroservice.NotificationService;
using Microsoft.EntityFrameworkCore;
using GranitMicroservice.NotificationService.Persistence;
using GranitMicroservice.ServiceDefaults;
using GranitMicroservice.Shared.Hosting.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ── Step 1 · Shared cross-cutting concerns ────────────────────────────────────
// AddSharedHostingAsync registers observability, health checks, JWT bearer auth,
// Redis cache, and the Wolverine outbox backed by PostgreSQL.
await builder.AddSharedHostingAsync();

// ── Step 1b · Infrastructure health checks ───────────────────────────────────
// This service is a pure message consumer — no HTTP endpoints — but it still
// needs readiness/startup probes so Aspire and Kubernetes know when it is ready
// to process messages from RabbitMQ.
builder.Services.AddHealthChecks()
    .AddGranitDbContextHealthCheck<NotificationServiceDbContext>()
    .AddGranitRedisHealthCheck();
builder.AddRabbitMqHealthCheck();

// ── Step 2 · Service-specific Granit modules ──────────────────────────────────
// NotificationServiceModule registers the Wolverine message handlers that react
// to integration events published by other services (e.g., OrderPlacedEvent →
// send email/SMS). This service is intentionally event-driven: it exposes no
// HTTP endpoints — all work is triggered asynchronously via RabbitMQ.
await builder.AddGranitAsync(granit => granit
    .AddModule<NotificationServiceModule>());

// ── Step 3 · EF Core DbContext ────────────────────────────────────────────────
// The notification service persists a delivery log (sent/failed notifications)
// for auditing and retry purposes. The Wolverine outbox guarantees at-least-once
// delivery — the delivery log lets operators detect and investigate duplicates.
// AddGranitDbContext uses ServiceLifetime.Scoped so that Granit interceptors
// (AuditedEntityInterceptor, …) can be resolved from the scoped provider.
builder.Services.AddGranitDbContext<NotificationServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("notification-db")));

WebApplication app = builder.Build();

// ── Step 4 · Granit middleware pipeline ───────────────────────────────────────
// UseGranitAsync runs module OnApplicationInitialization hooks (Wolverine
// startup, …). It does NOT apply EF Core migrations.
await app.UseGranitAsync();

// ── Step 4b · Database migrations (--migrate CLI flag) ───────────────────────
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

// ── Step 5 · Health endpoints ─────────────────────────────────────────────────
// MapGranitHealthChecks → /health/live (always 200), /health/ready (readiness
//   tag), /health/startup (startup tag) with structured JSON and stampede cache.
// No other endpoints: this service has no HTTP API surface.
app.MapGranitHealthChecks();

await app.RunAsync();
