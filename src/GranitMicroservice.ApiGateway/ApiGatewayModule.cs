using Granit.Bff;
using Granit.Bff.Endpoints;
using Granit.Bff.Yarp;
using Granit.Caching.StackExchangeRedis;
using Granit.Modularity;
using Granit.Observability;

namespace GranitMicroservice.ApiGateway;

[DependsOn(
    typeof(GranitBffModule),
    typeof(GranitBffEndpointsModule),
    typeof(GranitBffYarpModule),
    typeof(GranitCachingStackExchangeRedisModule),
    typeof(GranitObservabilityModule))]
public sealed class ApiGatewayModule : GranitModule;
