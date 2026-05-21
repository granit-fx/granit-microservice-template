using Granit.Authentication.JwtBearer;
using Granit.Caching.StackExchangeRedis;
using Granit.Caching.Vault;
using Granit.Modularity;
using Granit.Events.Wolverine;
using Granit.Http.Resilience;
using Granit.Persistence.EntityFrameworkCore.Hosting;
using Granit.Vault.HashiCorp;
using Granit.Wolverine.Postgresql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;

namespace GranitMicroservice.Shared.Hosting;

/// <summary>
/// Cross-cutting concerns shared by all microservices.
/// Domain-specific modules (Identity, Notifications, Authorization) are forbidden here.
/// </summary>
[DependsOn(
    typeof(GranitCachingStackExchangeRedisModule),
    typeof(GranitCachingVaultModule),
    typeof(GranitVaultHashiCorpModule),
    typeof(GranitEventsWolverineModule),
    typeof(GranitHttpResilienceModule),
    typeof(GranitAuthenticationJwtBearerModule),
    typeof(GranitPersistenceEntityFrameworkCoreHostingModule),
    typeof(GranitWolverinePostgresqlModule))]
public sealed class SharedHostingModule : GranitModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        if (context.Builder!.Environment.IsDevelopment())
        {
            // Solo mode disables multi-node agent coordination so Wolverine does not
            // hang at startup waiting for StopAgent ACKs from dead nodes left over by
            // previous Aspire runs — postgres has a persistent container volume, so
            // the wolverine_nodes table accumulates stale rows between AppHost restarts.
            context.Services.ConfigureWolverine(opts =>
                opts.Durability.Mode = DurabilityMode.Solo);
        }
    }
}
