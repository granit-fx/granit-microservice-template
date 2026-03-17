using Granit.Core.Extensions;
using Granit.Persistence.Interceptors;
using GranitMicroservice.NotificationService;
using GranitMicroservice.NotificationService.Persistence;
using GranitMicroservice.ServiceDefaults;
using GranitMicroservice.Shared.Hosting.Extensions;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// GranitWolverinePostgresqlModule reads WolverinePostgresql:TransportConnectionString.
// Aspire injects connection strings under ConnectionStrings:{name} — bridge the two.
builder.Configuration["WolverinePostgresql:TransportConnectionString"] =
    builder.Configuration.GetConnectionString("notification-db");

await builder.AddSharedHostingAsync();

await builder.AddGranitAsync(granit => granit
    .AddModule<NotificationServiceModule>());

builder.Services.AddDbContextFactory<NotificationServiceDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("notification-db"));
    options.AddInterceptors(
        sp.GetRequiredService<AuditedEntityInterceptor>());
});

WebApplication app = builder.Build();

await app.UseGranitAsync();

app.MapDefaultEndpoints();

await app.RunAsync();
