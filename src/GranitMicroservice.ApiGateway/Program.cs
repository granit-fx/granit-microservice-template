using System.Threading.RateLimiting;
using GranitMicroservice.ServiceDefaults;
using Scalar.AspNetCore;
using Yarp.ReverseProxy.Configuration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"];
        options.Audience = builder.Configuration["Authentication:Audience"];
        options.RequireHttpsMetadata = false; // dev only — Aspire manages TLS
    });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient("catalog", c => c.BaseAddress = new Uri("https+http://catalog-service"));
builder.Services.AddHttpClient("identity", c => c.BaseAddress = new Uri("https+http://identity-service"));

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("default", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 10,
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapReverseProxy();

// OpenAPI aggregation — proxy each service's spec
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

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "GranitMicroservice API";
    options.AddDocument("Catalog", "/openapi/catalog.json");
    options.AddDocument("Identity", "/openapi/identity.json");
});

await app.RunAsync();
