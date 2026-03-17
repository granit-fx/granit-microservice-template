using GranitMicroservice.CatalogService.Endpoints;
using Shouldly;

namespace GranitMicroservice.CatalogService.Tests.Endpoints;

public sealed class CatalogProductResponseTests
{
    [Fact]
    public void Should_create_response_with_all_properties()
    {
        var id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;

        var response = new CatalogProductResponse(
            id, "Widget", "A widget", 9.99m, categoryId, "Gadgets", createdAt, null);

        response.Id.ShouldBe(id);
        response.Name.ShouldBe("Widget");
        response.Description.ShouldBe("A widget");
        response.Price.ShouldBe(9.99m);
        response.CategoryId.ShouldBe(categoryId);
        response.CategoryName.ShouldBe("Gadgets");
        response.CreatedAt.ShouldBe(createdAt);
        response.ModifiedAt.ShouldBeNull();
    }

    [Fact]
    public void Should_support_record_equality()
    {
        var id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;

        var a = new CatalogProductResponse(id, "Widget", null, 9.99m, categoryId, null, createdAt, null);
        var b = new CatalogProductResponse(id, "Widget", null, 9.99m, categoryId, null, createdAt, null);

        a.ShouldBe(b);
    }
}
