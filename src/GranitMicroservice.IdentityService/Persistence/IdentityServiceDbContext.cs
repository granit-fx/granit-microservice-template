using Granit.Core.DataFiltering;
using Granit.Core.MultiTenancy;
using Granit.Identity.EntityFrameworkCore.DbContext;
using Granit.Identity.EntityFrameworkCore.Entities;
using Granit.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GranitMicroservice.IdentityService.Persistence;

public sealed class IdentityServiceDbContext(
    DbContextOptions<IdentityServiceDbContext> options,
    ICurrentTenant? currentTenant = null,
    IDataFilter? dataFilter = null) : DbContext(options), IUserCacheDbContext
{
    public DbSet<UserCacheEntry> UserCacheEntries => Set<UserCacheEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityServiceDbContext).Assembly);
        modelBuilder.ConfigureIdentityModule();
        modelBuilder.ApplyGranitConventions(currentTenant, dataFilter);
    }
}
