using Granit.Events;
using Granit.RateLimiting.AspNetCore;
using GranitMicroservice.CatalogService.Domain;
using GranitMicroservice.CatalogService.Persistence;
using GranitMicroservice.Shared.Events;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZiggyCreatures.Caching.Fusion;

namespace GranitMicroservice.CatalogService.Endpoints;

public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        // /products — domain prefix only. The "api" + "catalog" namespace lives
        // on the gateway side (YARP route /api/catalog/{**catch-all} → strip /api/catalog
        // → forwards /products/{...} to this service). Keeping the API surface free of
        // /api or /catalog avoids the double-prefix mismatch with the gateway routing.
        var group = routes.MapGroup("/products")
            .WithTags("Products")
            .RequireAuthorization();

        // api-default (SlidingWindow 1000 req/min) — read operations, generous quota
        group.MapGet("/", GetProducts).RequireGranitRateLimiting("api-default");
        group.MapGet("/{id:guid}", GetProduct).RequireGranitRateLimiting("api-default");

        // api-write (TokenBucket 50 tokens, +10/10s) — write operations, burst-controlled
        group.MapPost("/", CreateProduct).RequireGranitRateLimiting("api-write");
        group.MapPut("/{id:guid}", UpdateProduct).RequireGranitRateLimiting("api-write");
        group.MapDelete("/{id:guid}", DeleteProduct).RequireGranitRateLimiting("api-write");

        return group;
    }

    private static async Task<Ok<List<CatalogProductResponse>>> GetProducts(
        CatalogDbContext db,
        CancellationToken cancellationToken,
        int skip = 0,
        int take = 20)
    {
        var products = await db.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(take)
            .Select(p => MapToResponse(p))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(products);
    }

    private static async Task<Results<Ok<CatalogProductResponse>, ProblemHttpResult>> GetProduct(
        Guid id,
        CatalogDbContext db,
        IFusionCache cache,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"product:{id}";
        var cached = await cache.TryGetAsync<CatalogProductResponse>(cacheKey, token: cancellationToken);

        if (cached.HasValue)
            return TypedResults.Ok(cached.Value);

        var product = await db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product is null)
        {
            return TypedResults.Problem(
                detail: $"Product with id '{id}' not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        var response = MapToResponse(product);
        await cache.SetAsync(cacheKey, response, token: cancellationToken);
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Created<CatalogProductResponse>, ProblemHttpResult>> CreateProduct(
        CreateCatalogProductRequest request,
        CatalogDbContext db,
        IDistributedEventBus eventBus,
        CancellationToken cancellationToken)
    {
        var categoryExists = await db.Categories
            .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            return TypedResults.Problem(
                detail: $"Category with id '{request.CategoryId}' not found.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId,
        };

        db.Products.Add(product);
        await db.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(
            new CatalogProductCreatedEvent(product.Id, product.Name, product.Price),
            cancellationToken);

        var created = await db.Products
            .Include(p => p.Category)
            .FirstAsync(p => p.Id == product.Id, cancellationToken);

        return TypedResults.Created(
            $"/api/products/{created.Id}",
            MapToResponse(created));
    }

    private static async Task<Results<Ok<CatalogProductResponse>, ProblemHttpResult>> UpdateProduct(
        Guid id,
        UpdateCatalogProductRequest request,
        CatalogDbContext db,
        IDistributedEventBus eventBus,
        IFusionCache cache,
        CancellationToken cancellationToken)
    {
        var product = await db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product is null)
        {
            return TypedResults.Problem(
                detail: $"Product with id '{id}' not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;

        await db.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(
            new CatalogProductUpdatedEvent(product.Id, product.Name, product.Price),
            cancellationToken);

        await cache.RemoveAsync($"product:{id}", token: cancellationToken);

        return TypedResults.Ok(MapToResponse(product));
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> DeleteProduct(
        Guid id,
        CatalogDbContext db,
        IFusionCache cache,
        CancellationToken cancellationToken)
    {
        var product = await db.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product is null)
        {
            return TypedResults.Problem(
                detail: $"Product with id '{id}' not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        db.Products.Remove(product);
        await db.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync($"product:{id}", token: cancellationToken);

        return TypedResults.NoContent();
    }

    private static CatalogProductResponse MapToResponse(Product product) =>
        new(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.CategoryId,
            product.Category?.Name,
            product.CreatedAt,
            product.ModifiedAt);

}
