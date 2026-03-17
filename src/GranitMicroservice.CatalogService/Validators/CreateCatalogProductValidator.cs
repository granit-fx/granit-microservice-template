using FluentValidation;
using GranitMicroservice.CatalogService.Endpoints;

namespace GranitMicroservice.CatalogService.Validators;

public sealed class CreateCatalogProductValidator : AbstractValidator<CreateCatalogProductRequest>
{
    public CreateCatalogProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.CategoryId)
            .NotEmpty();
    }
}
