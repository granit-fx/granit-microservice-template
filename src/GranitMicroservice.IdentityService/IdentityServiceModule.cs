using Granit.Authentication.JwtBearer;
using Granit.Authentication.JwtBearer.Keycloak;
using Granit.Authorization;
using Granit.Modularity;
using Granit.Identity;
using Granit.Identity.Endpoints;
using Granit.Identity.Federated.EntityFrameworkCore;
using Granit.Identity.Federated.Keycloak;
using Granit.Persistence.EntityFrameworkCore;
using Granit.Persistence.EntityFrameworkCore.Hosting;
using GranitMicroservice.IdentityService.Persistence;

namespace GranitMicroservice.IdentityService;

[DependsOn(
    typeof(GranitAuthenticationJwtBearerKeycloakModule),
    typeof(GranitAuthorizationModule),
    typeof(GranitIdentityModule),
    typeof(GranitIdentityEndpointsModule),
    typeof(GranitIdentityFederatedEntityFrameworkCoreModule),
    typeof(GranitIdentityFederatedKeycloakModule),
    typeof(GranitAuthenticationJwtBearerModule),
    typeof(GranitPersistenceEntityFrameworkCoreModule))]
public sealed class IdentityServiceModule : GranitModule, IMigratableModule<IdentityServiceDbContext>;
