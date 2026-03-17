namespace GranitMicroservice.CatalogService.Endpoints;

public sealed record CreateCatalogProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId);

public sealed record UpdateCatalogProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId);
