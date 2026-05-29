using Granit.Auditing.EntityFrameworkCore.Extensions;
using Granit.DataFiltering;
using Granit.Identity.EntityFrameworkCore.Extensions;
using Granit.Identity.Federated.EntityFrameworkCore.Extensions;
using Granit.MultiTenancy;
using Granit.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GranitMicroservice.IdentityService.Persistence;

public sealed class IdentityServiceDbContext(
    DbContextOptions<IdentityServiceDbContext> options,
    ICurrentTenant currentTenant,
    IDataFilter? dataFilter = null)
    : GranitDbContext(options, currentTenant, dataFilter)
{
    protected override void OnGranitModelCreating(ModelBuilder modelBuilder)
    {
        // Owns the migrations for the federated identity cache
        // (identity_user_cache_entries), the canonical User aggregate, and the audit
        // trail. Runtime reads/writes go through the stores' own DbContexts — the
        // internal IdentityFederatedHostDbContext, IdentityHostDbContext and
        // AuditingHostDbContext, registered by AddGranitIdentityFederatedEntityFrameworkCore,
        // AddGranitIdentityEntityFrameworkCore and AddGranitAuditingEntityFrameworkCore
        // respectively — which map the same physical tables. Calling the
        // Configure*Module() helpers here keeps schema ownership on this single
        // host-migrated context (see IdentityServiceModule : IMigratableModule).
        modelBuilder.ConfigureIdentityModule();
        modelBuilder.ConfigureGranitIdentityModule();
        modelBuilder.ConfigureAuditingModule();
    }
}
