using Granit.Events;

namespace GranitMicroservice.Shared.Events;

/// <summary>
/// Published when a new product is created in the CatalogService.
/// </summary>
public sealed record CatalogProductCreatedEvent(
    Guid ProductId,
    string Name,
    decimal Price) : IIntegrationEvent;
