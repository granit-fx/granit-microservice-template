using Granit.Auditing;
using Granit.Bff.Endpoints.Extensions;
using Granit.Bff.Yarp.Extensions;
using Granit.Extensions;
using Granit.Http.Cors.Extensions;
using GranitMicroservice.ApiGateway;
using GranitMicroservice.ApiGateway.Internal;
using GranitMicroservice.ServiceDefaults;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ── Step 1 · Service defaults (Aspire) ────────────────────────────────────────
// AddServiceDefaults registers the OpenTelemetry pipeline, health checks, and
// Aspire service discovery.
builder.AddServiceDefaults();

// ── Step 1b · Granit module system ────────────────────────────────────────────
// ApiGatewayModule loads the BFF stack (GranitBffModule → IBffTokenStore,
// GranitBffYarpModule → YARP transforms, GranitCachingStackExchangeRedisModule
// → IDistributedCache backed by Redis for server-side token storage).
await builder.AddGranitAsync<ApiGatewayModule>();

// GranitBffEndpointsModule pulls in GranitAuditingModule, which registers a
// background AuditingCleanupWorker that requires IAuditingCleaner. The gateway
// has no audit persistence — install Granit.Auditing.EntityFrameworkCore and
// call AddGranitAuditingEntityFrameworkCore to enable retention, and remove
// this stub. Until then, audit writes from BFF endpoints are silently dropped
// (they resolve IAuditingWriter optionally) and the cleanup worker no-ops.
builder.Services.TryAddSingleton<IAuditingCleaner, NullAuditingCleaner>();

// ── Step 2 · Authentication ───────────────────────────────────────────────────
// Required by Granit.Http.ApiDocumentation transformers (loaded transitively)
// and by the BFF cookie-based session validation.
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// ── Step 2b · CORS ────────────────────────────────────────────────────────────
// AddGranitCors reads Cors:AllowedOrigins from configuration and registers
// a default policy. In BFF mode, CORS is less critical (same-origin cookies),
// but still needed for health probes and service-to-service preflight.
builder.AddGranitCors();

// ── Step 3 · BFF reverse proxy ──────────────────────────────────────────────
// Replaces the previous JWT Bearer + inline YARP setup with a BFF pattern.
// The gateway handles OIDC authorization code flow with Keycloak, stores tokens
// server-side in Redis (via IDistributedCache), and injects Bearer tokens into
// proxied requests. The SPA never sees or stores tokens — only an HttpOnly cookie.
//
// Route flow example:
//   Client → GET /api/catalog/products (with session cookie)
//   BFF reads cookie → resolves tokens from cache → injects Authorization: Bearer
//   YARP removes the /api/catalog prefix
//   → forwarded as GET /products to catalog-service (with Bearer token)
//
// YARP routes are loaded from appsettings.json ReverseProxy section.
// Each route with Granit.Bff.RequireAuth=true gets automatic token injection.
builder.AddGranitBffYarp();

// ── Step 4 · OpenAPI aggregation clients ─────────────────────────────────────
// Named HttpClients proxy each service's OpenAPI spec for a unified Scalar UI.
builder.Services.AddOpenApi();
builder.Services.AddHttpClient("catalog", c => c.BaseAddress = new Uri("https+http://catalog-service"));
builder.Services.AddHttpClient("identity", c => c.BaseAddress = new Uri("https+http://identity-service"));

WebApplication app = builder.Build();
await app.UseGranitAsync();

// ── Step 5 · Middleware pipeline ──────────────────────────────────────────────
// Order matters:
//   1. Health endpoints — must respond even before auth
//   2. CORS            → preflight OPTIONS handled before cookie validation
//   3. BFF YARP        → reads session cookie, injects token, proxies to services
//   4. BFF endpoints   → /bff/login, /bff/logout, /bff/user, /bff/sessions
app.MapDefaultEndpoints();

app.UseCors();
app.UseGranitBffYarp();
app.MapGranitBff();

// ── Step 6 · OpenAPI spec passthrough ─────────────────────────────────────────
app.MapGet("/openapi/catalog.json", async (IHttpClientFactory factory) =>
{
    HttpClient client = factory.CreateClient("catalog");
    string spec = await client.GetStringAsync("/openapi/v1.json");
    return Results.Content(spec, "application/json");
}).ExcludeFromDescription();

app.MapGet("/openapi/identity.json", async (IHttpClientFactory factory) =>
{
    HttpClient client = factory.CreateClient("identity");
    string spec = await client.GetStringAsync("/openapi/v1.json");
    return Results.Content(spec, "application/json");
}).ExcludeFromDescription();

// ── Step 7 · Unified Scalar UI ────────────────────────────────────────────────
// The gateway aggregates per-service OpenAPI docs into a single Scalar UI, so it
// can't use Granit.Http.ApiDocumentation (which is single-document by design).
// We register the same OAuth2 Authorization Code + PKCE flow manually — the
// security scheme name "OAuth2" matches what each service's OpenAPI document
// declares (OAuth2SecuritySchemeTransformer in Granit.Http.ApiDocumentation),
// so the "Authorize" button drives a real Keycloak login round-trip.
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "GranitMicroservice API";
    options.AddDocument("Catalog", "/openapi/catalog.json");
    options.AddDocument("Identity", "/openapi/identity.json");

    string authorizationUrl = builder.Configuration["Http:ApiDocumentation:OAuth2:AuthorizationUrl"]!;
    string tokenUrl = builder.Configuration["Http:ApiDocumentation:OAuth2:TokenUrl"]!;
    string clientId = builder.Configuration["Http:ApiDocumentation:OAuth2:ClientId"]!;
    string[] scopes = builder.Configuration.GetSection("Http:ApiDocumentation:OAuth2:Scopes").Get<string[]>()
        ?? ["openid"];

    options.AddAuthorizationCodeFlow("OAuth2", flow => flow
        .WithAuthorizationUrl(authorizationUrl)
        .WithTokenUrl(tokenUrl)
        .WithClientId(clientId)
        .WithSelectedScopes(scopes)
        .WithPkce(Pkce.Sha256));
});

await app.RunAsync();
