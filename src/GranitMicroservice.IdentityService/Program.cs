using Granit.Core.Extensions;
using Granit.Identity.EntityFrameworkCore.Extensions;
using Granit.Persistence.Interceptors;
using GranitMicroservice.IdentityService;
using GranitMicroservice.IdentityService.Persistence;
using Granit.Identity.Endpoints.Extensions;
using GranitMicroservice.ServiceDefaults;
using GranitMicroservice.Shared.Hosting.Extensions;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

await builder.AddSharedHostingAsync();

await builder.AddGranitAsync(granit => granit
    .AddModule<IdentityServiceModule>());

builder.Services.AddDbContextFactory<IdentityServiceDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("identity-db"));
    options.AddInterceptors(
        sp.GetRequiredService<AuditedEntityInterceptor>());
});

builder.Services.AddGranitIdentityEntityFrameworkCore<IdentityServiceDbContext>();

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

await app.UseGranitAsync();

app.MapDefaultEndpoints();
app.MapOpenApi();
app.MapIdentityUserCacheEndpoints();
app.MapScalarApiReference();

await app.RunAsync();
