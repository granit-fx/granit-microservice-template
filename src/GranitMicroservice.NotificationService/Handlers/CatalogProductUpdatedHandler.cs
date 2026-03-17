using GranitMicroservice.Shared.Events;
using Microsoft.Extensions.Logging;

namespace GranitMicroservice.NotificationService.Handlers;

/// <summary>
/// Handles <see cref="CatalogProductUpdatedEvent"/> from the CatalogService.
/// Triggers a notification when a product is updated in the catalog.
/// </summary>
public sealed partial class CatalogProductUpdatedHandler(
    ILogger<CatalogProductUpdatedHandler> logger)
{
    public async Task HandleAsync(
        CatalogProductUpdatedEvent @event,
        CancellationToken cancellationToken)
    {
        LogProductUpdatedNotification(@event.ProductId, @event.Name);

        // TODO: Phase 3 — integrate with Granit.Notifications to send notifications
        await Task.CompletedTask;
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Notification triggered for updated product {ProductId}: {ProductName}")]
    private partial void LogProductUpdatedNotification(Guid productId, string productName);
}
