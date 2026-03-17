using Granit.Core.Events;

namespace GranitMicroservice.Shared.Events;

/// <summary>
/// Published when an existing product is updated in the CatalogService.
/// </summary>
public sealed record CatalogProductUpdatedEvent(
    Guid ProductId,
    string Name,
    decimal Price) : IIntegrationEvent;
