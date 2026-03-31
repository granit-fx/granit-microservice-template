using Granit.Authentication.JwtBearer;
using Granit.Caching.StackExchangeRedis;
using Granit.Modularity;
using Granit.Events.Wolverine;
using Granit.Http.Resilience;
using Granit.Persistence.EntityFrameworkCore.Hosting;
using Granit.Wolverine.Postgresql;

namespace GranitMicroservice.Shared.Hosting;

/// <summary>
/// Cross-cutting concerns shared by all microservices.
/// Domain-specific modules (Identity, Notifications, Authorization) are forbidden here.
/// </summary>
[DependsOn(
    typeof(GranitCachingStackExchangeRedisModule),
    typeof(GranitEventsWolverineModule),
    typeof(GranitHttpResilienceModule),
    typeof(GranitAuthenticationJwtBearerModule),
    typeof(GranitPersistenceEntityFrameworkCoreHostingModule),
    typeof(GranitWolverinePostgresqlModule))]
public sealed class SharedHostingModule : GranitModule;
