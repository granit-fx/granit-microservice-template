namespace GranitMicroservice.CatalogService.Endpoints;

public sealed record CatalogProductResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    string? CategoryName,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ModifiedAt);
