using Granit.DataFiltering;
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityServiceDbContext).Assembly);
        modelBuilder.ConfigureIdentityModule();
    }
}
