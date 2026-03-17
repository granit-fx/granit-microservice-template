using System.Security.Claims;
using System.Threading.RateLimiting;
using Granit.Http.Cors.Extensions;
using GranitMicroservice.ServiceDefaults;
using Scalar.AspNetCore;
using Yarp.ReverseProxy.Configuration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ── Step 1 · Service defaults (Aspire) ────────────────────────────────────────
// AddServiceDefaults registers the OpenTelemetry pipeline, health checks, and
// Aspire service discovery. The gateway does NOT use AddSharedHostingAsync
// because it carries no business logic and needs no Wolverine/Redis/outbox.
builder.AddServiceDefaults();

// ── Step 1b · CORS ────────────────────────────────────────────────────────────
// AddGranitCors reads Cors:AllowedOrigins from configuration and registers
// a default policy. Wildcard (*) is rejected at startup in non-development
// environments (ISO 27001 compliance). AllowAnyHeader + AllowAnyMethod is
// intentional — origin restriction is the relevant control for REST APIs.
// app.UseCors() is placed before authentication in the middleware pipeline
// so preflight OPTIONS requests are handled without needing a valid token.
builder.AddGranitCors();


// ── Step 2 · YARP reverse proxy ───────────────────────────────────────────────
// YARP (Yet Another Reverse Proxy) routes inbound requests from external clients
// to the correct backend service. Routes are loaded in-memory here for simplicity;
// production setups can load them from configuration or a database at runtime.
//
// Route flow example:
//   Client → GET /api/catalog/products
//   Gateway removes the /api/catalog prefix
//   → forwarded as GET /products to catalog-service
//
// "https+http://..." tells Aspire to prefer HTTPS but fall back to HTTP.
// Aspire injects the actual service addresses via service discovery.
builder.Services.AddReverseProxy()
    .LoadFromMemory(
    [
        new RouteConfig
        {
            RouteId = "catalog",
            ClusterId = "catalog",
            AuthorizationPolicy = "default",
            Match = new RouteMatch { Path = "/api/catalog/{**catch-all}" },
            Transforms =
            [
                new Dictionary<string, string> { ["PathRemovePrefix"] = "/api/catalog" },
            ],
        },
        new RouteConfig
        {
            RouteId = "identity",
            ClusterId = "identity",
            AuthorizationPolicy = "default",
            Match = new RouteMatch { Path = "/api/identity/{**catch-all}" },
            Transforms =
            [
                new Dictionary<string, string> { ["PathRemovePrefix"] = "/api/identity" },
            ],
        },
    ],
    [
        new ClusterConfig
        {
            ClusterId = "catalog",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                ["default"] = new() { Address = "https+http://catalog-service" },
            },
        },
        new ClusterConfig
        {
            ClusterId = "identity",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                ["default"] = new() { Address = "https+http://identity-service" },
            },
        },
    ]);

// ── Step 3 · Authentication ───────────────────────────────────────────────────
// The gateway validates JWT Bearer tokens issued by Keycloak before forwarding
// requests. Individual services trust the gateway and do not re-validate tokens
// (defense-in-depth: services are not exposed directly outside the cluster).
// RequireHttpsMetadata=false is intentional for dev — Aspire manages TLS.
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"];
        options.Audience = builder.Configuration["Authentication:Audience"];
        options.RequireHttpsMetadata = false; // dev only — Aspire manages TLS
    });

builder.Services.AddAuthorization();

// ── Step 4 · OpenAPI aggregation clients ─────────────────────────────────────
// Named HttpClients are used in Steps 7–8 to proxy each service's OpenAPI spec
// and surface them in a unified Scalar UI (one explorer for the whole platform).
builder.Services.AddOpenApi();
builder.Services.AddHttpClient("catalog", c => c.BaseAddress = new Uri("https+http://catalog-service"));
builder.Services.AddHttpClient("identity", c => c.BaseAddress = new Uri("https+http://identity-service"));

// ── Step 5 · Rate limiting ────────────────────────────────────────────────────
// Sliding-window rate limiter partitioned per authenticated user (JWT sub claim).
// Falls back to client IP for unauthenticated requests (public endpoints, probes).
// HTTP 429 is returned when the limit is exceeded (standard REST convention).
// Per-user partitioning prevents a single compromised or abusive account from
// degrading service for all other users sharing the same IP (NAT, VPN, proxies).
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("default", context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? context.Connection.RemoteIpAddress?.ToString()
                          ?? "anonymous",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6,
                QueueLimit = 0,
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

WebApplication app = builder.Build();

// ── Step 6 · Middleware pipeline ──────────────────────────────────────────────
// Order matters:
//   1. Health endpoints — must respond even before auth/rate-limiting
//   2. CORS            → preflight OPTIONS handled before token validation
//   3. Authentication  → populates HttpContext.User
//   4. Authorization   → enforces policies defined on YARP routes
//   5. RateLimiter     → applied after auth so per-user limits are possible
//   6. ReverseProxy    → forwards the request to the target cluster
app.MapDefaultEndpoints();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapReverseProxy();

// ── Step 7 · OpenAPI spec passthrough ─────────────────────────────────────────
// Each endpoint below fetches the raw OpenAPI JSON from the corresponding service
// and serves it under a stable gateway URL. ExcludeFromDescription() hides these
// plumbing routes from the gateway's own spec (they belong to each service spec).
app.MapGet("/openapi/catalog.json", async (IHttpClientFactory factory) =>
{
    var client = factory.CreateClient("catalog");
    var spec = await client.GetStringAsync("/openapi/v1.json");
    return Results.Content(spec, "application/json");
}).ExcludeFromDescription();

app.MapGet("/openapi/identity.json", async (IHttpClientFactory factory) =>
{
    var client = factory.CreateClient("identity");
    var spec = await client.GetStringAsync("/openapi/v1.json");
    return Results.Content(spec, "application/json");
}).ExcludeFromDescription();

// ── Step 8 · Unified Scalar UI ────────────────────────────────────────────────
// A single interactive explorer at /scalar aggregates all service specs.
// Developers can browse and test every API from one place, authenticated
// through the gateway — exactly as a real client would interact.
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "GranitMicroservice API";
    options.AddDocument("Catalog", "/openapi/catalog.json");
    options.AddDocument("Identity", "/openapi/identity.json");
});

await app.RunAsync();
