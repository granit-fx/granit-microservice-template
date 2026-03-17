using Granit.Authentication.JwtBearer;
using Granit.Caching.StackExchangeRedis;
using Granit.Core.Modularity;
using Granit.EventBus.Wolverine;
using Granit.Http.Resilience;
using Granit.Wolverine.Postgresql;

namespace GranitMicroservice.Shared.Hosting;

/// <summary>
/// Cross-cutting concerns shared by all microservices.
/// Domain-specific modules (Identity, Notifications, Authorization) are forbidden here.
/// </summary>
[DependsOn(
    typeof(GranitCachingRedisModule),
    typeof(GranitEventBusWolverineModule),
    typeof(GranitHttpResilienceModule),
    typeof(GranitJwtBearerModule),
    typeof(GranitWolverinePostgresqlModule))]
public sealed class SharedHostingModule : GranitModule;
