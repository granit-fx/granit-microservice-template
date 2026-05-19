using Granit.DataFiltering;
using Granit.MultiTenancy;
using Granit.Persistence.EntityFrameworkCore;
using GranitMicroservice.CatalogService.Domain;
using Microsoft.EntityFrameworkCore;

namespace GranitMicroservice.CatalogService.Persistence;

public sealed class CatalogDbContext(
    DbContextOptions<CatalogDbContext> options,
    ICurrentTenant currentTenant,
    IDataFilter? dataFilter = null) : GranitDbContext(options, currentTenant, dataFilter)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnGranitModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
