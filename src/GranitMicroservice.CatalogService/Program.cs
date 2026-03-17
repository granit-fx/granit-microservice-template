using Granit.Core.Extensions;
using Granit.Persistence.Interceptors;
using GranitMicroservice.CatalogService;
using GranitMicroservice.CatalogService.Endpoints;
using GranitMicroservice.CatalogService.Persistence;
using GranitMicroservice.ServiceDefaults;
using GranitMicroservice.Shared.Hosting.Extensions;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Map Aspire connection string to Wolverine PostgreSQL outbox config
builder.Configuration["WolverinePostgresql:ConnectionString"] =
    builder.Configuration.GetConnectionString("catalog-db");

await builder.AddSharedHostingAsync();

await builder.AddGranitAsync(granit => granit
    .AddModule<CatalogServiceModule>());

builder.Services.AddDbContextFactory<CatalogDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("catalog-db"));
    options.AddInterceptors(
        sp.GetRequiredService<AuditedEntityInterceptor>(),
        sp.GetRequiredService<SoftDeleteInterceptor>());
});

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

await app.UseGranitAsync();

app.MapDefaultEndpoints();
app.MapOpenApi();
app.MapScalarApiReference();
app.MapProductEndpoints();

await app.RunAsync();
