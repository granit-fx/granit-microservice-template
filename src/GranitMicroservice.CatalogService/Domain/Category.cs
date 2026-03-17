using Granit.Core.Domain;

namespace GranitMicroservice.CatalogService.Domain;

public sealed class Category : AuditedEntity
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public List<Product> Products { get; set; } = [];
}
