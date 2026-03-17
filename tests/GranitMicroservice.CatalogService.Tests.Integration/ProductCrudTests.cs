using GranitMicroservice.CatalogService.Domain;
using Microsoft.EntityFrameworkCore;
using Shouldly;


namespace GranitMicroservice.CatalogService.Tests.Integration;

public sealed class ProductCrudTests(CatalogDbFixture fixture) : IClassFixture<CatalogDbFixture>
{
    [Fact]
    public async Task Should_create_and_retrieve_product()
    {
        await using var db = fixture.CreateDbContext();

        var category = new Category { Name = "Electronics" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var product = new Product
        {
            Name = "Laptop",
            Description = "A fast laptop",
            Price = 999.99m,
            CategoryId = category.Id,
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        var retrieved = await db.Products
            .Include(p => p.Category)
            .FirstAsync(p => p.Id == product.Id);

        retrieved.Name.ShouldBe("Laptop");
        retrieved.Price.ShouldBe(999.99m);
        retrieved.Category.ShouldNotBeNull();
        retrieved.Category!.Name.ShouldBe("Electronics");
    }

    [Fact]
    public async Task Should_update_product()
    {
        await using var db = fixture.CreateDbContext();

        var category = new Category { Name = "Books" };
        db.Categories.Add(category);

        var product = new Product
        {
            Name = "Old Title",
            Price = 10m,
            CategoryId = category.Id,
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        product.Name = "New Title";
        product.Price = 15m;
        await db.SaveChangesAsync();

        var retrieved = await db.Products.FirstAsync(p => p.Id == product.Id);

        retrieved.Name.ShouldBe("New Title");
        retrieved.Price.ShouldBe(15m);
    }

    [Fact]
    public async Task Should_delete_product()
    {
        await using var db = fixture.CreateDbContext();

        var category = new Category { Name = "Toys" };
        db.Categories.Add(category);

        var product = new Product
        {
            Name = "Teddy Bear",
            Price = 25m,
            CategoryId = category.Id,
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        db.Products.Remove(product);
        await db.SaveChangesAsync();

        var exists = await db.Products.AnyAsync(p => p.Id == product.Id);
        exists.ShouldBeFalse();
    }
}
