namespace GranitMicroservice.CatalogService.Cache;

/// <summary>
/// Redis cache entry for a single product.
/// Key format: <c>{Cache:KeyPrefix}:Product:{id}</c> (the "CacheItem" suffix is stripped by convention).
/// TTL is governed by <c>Cache:DefaultAbsoluteExpirationRelativeToNow</c> (default: 1 h).
/// </summary>
public sealed class ProductCacheItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public Guid CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? ModifiedAt { get; init; }
}
