using Granit.DataFiltering;
using Granit.Identity.EntityFrameworkCore.Extensions;
using Granit.Identity.Federated.Domain;
using Granit.Identity.Federated.EntityFrameworkCore.DbContext;
using Granit.MultiTenancy;
using Granit.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GranitMicroservice.IdentityService.Persistence;

public sealed class IdentityServiceDbContext(
    DbContextOptions<IdentityServiceDbContext> options,
    ICurrentTenant currentTenant,
    IDataFilter? dataFilter = null)
    : GranitDbContext(options, currentTenant, dataFilter), IUserCacheDbContext
{
    public DbSet<FederatedIdentity> FederatedIdentities => Set<FederatedIdentity>();

    protected override void OnGranitModelCreating(ModelBuilder modelBuilder)
    {
        // Owns the migrations for both the federated identity cache
        // (FederatedIdentity) and the canonical User aggregate. The runtime
        // writes to the User table go through IdentityDbContext registered
        // by AddGranitIdentityEntityFrameworkCore — both contexts map the
        // same physical table.
        modelBuilder.ConfigureIdentityModule();
        modelBuilder.ConfigureGranitIdentityModule();
    }
}
