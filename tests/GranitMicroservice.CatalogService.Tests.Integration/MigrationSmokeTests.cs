using Shouldly;

namespace GranitMicroservice.CatalogService.Tests.Integration;

public sealed class MigrationSmokeTests(CatalogDbFixture fixture) : IClassFixture<CatalogDbFixture>
{
    [Fact]
    public async Task Should_apply_schema_successfully()
    {
        await using var db = fixture.CreateDbContext();

        var canConnect = await db.Database.CanConnectAsync();

        canConnect.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_have_products_table()
    {
        await using var db = fixture.CreateDbContext();

        var count = await db.Products.CountAsync();

        count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_have_categories_table()
    {
        await using var db = fixture.CreateDbContext();

        var count = await db.Categories.CountAsync();

        count.ShouldBeGreaterThanOrEqualTo(0);
    }
}
