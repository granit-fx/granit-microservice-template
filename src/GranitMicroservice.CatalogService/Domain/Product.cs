using Granit.Domain;

namespace GranitMicroservice.CatalogService.Domain;

public sealed class Product : FullAuditedEntity
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public Guid CategoryId { get; set; }

    public Category? Category { get; set; }
}
