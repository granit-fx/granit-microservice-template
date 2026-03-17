using Bogus;
using FluentValidation.TestHelper;
using GranitMicroservice.CatalogService.Endpoints;
using GranitMicroservice.CatalogService.Validators;

namespace GranitMicroservice.CatalogService.Tests.Validators;

public sealed class UpdateCatalogProductValidatorTests
{
    private readonly UpdateCatalogProductValidator _validator = new();

    private readonly Faker<UpdateCatalogProductRequest> _validRequestFaker = new Faker<UpdateCatalogProductRequest>()
        .CustomInstantiator(f => new UpdateCatalogProductRequest(
            f.Commerce.ProductName(),
            f.Commerce.ProductDescription(),
            decimal.Parse(f.Commerce.Price()),
            f.Random.Guid()));

    [Fact]
    public void Should_pass_for_valid_request()
    {
        var request = _validRequestFaker.Generate();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_name_is_empty()
    {
        var request = _validRequestFaker.Generate() with { Name = "" };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_fail_when_price_is_negative()
    {
        var request = _validRequestFaker.Generate() with { Price = -0.01m };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Should_fail_when_category_id_is_empty()
    {
        var request = _validRequestFaker.Generate() with { CategoryId = Guid.Empty };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }
}
