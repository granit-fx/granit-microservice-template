using GranitMicroservice.CatalogService.Domain;
using Shouldly;

namespace GranitMicroservice.CatalogService.Tests.Domain;

public sealed class ProductTests
{
    [Fact]
    public void Should_create_product_with_required_properties()
    {
        var categoryId = Guid.NewGuid();

        var product = new Product
        {
            Name = "Test Product",
            Description = "A test product",
            Price = 19.99m,
            CategoryId = categoryId,
        };

        product.Name.ShouldBe("Test Product");
        product.Description.ShouldBe("A test product");
        product.Price.ShouldBe(19.99m);
        product.CategoryId.ShouldBe(categoryId);
    }

    [Fact]
    public void Should_allow_null_description()
    {
        var product = new Product
        {
            Name = "Minimal Product",
            Price = 0m,
            CategoryId = Guid.NewGuid(),
        };

        product.Description.ShouldBeNull();
    }
}
