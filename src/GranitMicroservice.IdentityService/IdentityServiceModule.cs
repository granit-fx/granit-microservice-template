using Granit.Authentication.JwtBearer;
using Granit.Authentication.Keycloak;
using Granit.Authorization;
using Granit.Core.Modularity;
using Granit.Identity;
using Granit.Identity.Endpoints;
using Granit.Identity.EntityFrameworkCore;
using Granit.Identity.Keycloak;
using Granit.Persistence;
using Granit.Persistence.Migrations;

namespace GranitMicroservice.IdentityService;

[DependsOn(
    typeof(GranitAuthenticationKeycloakModule),
    typeof(GranitAuthorizationModule),
    typeof(GranitIdentityModule),
    typeof(GranitIdentityEndpointsModule),
    typeof(GranitIdentityEntityFrameworkCoreModule),
    typeof(GranitIdentityKeycloakModule),
    typeof(GranitJwtBearerModule),
    typeof(GranitPersistenceModule),
    typeof(GranitPersistenceMigrationsModule))]
public sealed class IdentityServiceModule : GranitModule;
