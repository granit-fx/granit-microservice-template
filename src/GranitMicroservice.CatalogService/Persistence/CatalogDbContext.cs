using Granit.DataFiltering;
using Granit.MultiTenancy;
using Granit.Persistence.EntityFrameworkCore.Extensions;
using GranitMicroservice.CatalogService.Domain;
using Microsoft.EntityFrameworkCore;

namespace GranitMicroservice.CatalogService.Persistence;

public sealed class CatalogDbContext(
    DbContextOptions<CatalogDbContext> options,
    ICurrentTenant? currentTenant = null,
    IDataFilter? dataFilter = null) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        modelBuilder.ApplyGranitConventions(currentTenant, dataFilter);
    }
}
