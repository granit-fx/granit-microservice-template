using Granit.Persistence.DataSeeding;
using GranitMicroservice.CatalogService.Domain;
using Microsoft.EntityFrameworkCore;

namespace GranitMicroservice.CatalogService.Persistence;

/// <summary>
/// Seeds demo categories and products for development and local exploration.
/// Idempotent: no-op if any product already exists.
/// Called automatically by <c>UseGranitAsync()</c> on startup.
/// </summary>
internal sealed class CatalogDataSeedContributor(CatalogDbContext dbContext) : IDataSeedContributor
{
    public async Task SeedAsync(DataSeedContext context, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Products.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        Category electronics = new() { Id = new Guid("10000000-0000-0000-0000-000000000001"), Name = "Electronics", Description = "Devices and accessories" };
        Category clothing   = new() { Id = new Guid("10000000-0000-0000-0000-000000000002"), Name = "Clothing",    Description = "Apparel and footwear" };
        Category books      = new() { Id = new Guid("10000000-0000-0000-0000-000000000003"), Name = "Books",       Description = "Print and digital books" };

        dbContext.Categories.AddRange(electronics, clothing, books);

        dbContext.Products.AddRange(
            new Product { Id = new Guid("20000000-0000-0000-0000-000000000001"), Name = "Wireless Headphones",  Price = 79.99m,  CategoryId = electronics.Id },
            new Product { Id = new Guid("20000000-0000-0000-0000-000000000002"), Name = "Mechanical Keyboard",  Price = 129.99m, CategoryId = electronics.Id },
            new Product { Id = new Guid("20000000-0000-0000-0000-000000000003"), Name = "USB-C Hub",            Price = 39.99m,  CategoryId = electronics.Id },
            new Product { Id = new Guid("20000000-0000-0000-0000-000000000004"), Name = "Running Shoes",        Price = 89.99m,  CategoryId = clothing.Id },
            new Product { Id = new Guid("20000000-0000-0000-0000-000000000005"), Name = "Merino Wool Sweater",  Price = 69.99m,  CategoryId = clothing.Id },
            new Product { Id = new Guid("20000000-0000-0000-0000-000000000006"), Name = "Clean Code",          Price = 34.99m,  CategoryId = books.Id },
            new Product { Id = new Guid("20000000-0000-0000-0000-000000000007"), Name = "Domain-Driven Design", Price = 44.99m,  CategoryId = books.Id }
        );

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
