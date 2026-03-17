using GranitMicroservice.Shared.Events;
using Microsoft.Extensions.Logging;

namespace GranitMicroservice.NotificationService.Handlers;

/// <summary>
/// Handles <see cref="CatalogProductCreatedEvent"/> from the CatalogService.
/// Triggers a notification when a new product is added to the catalog.
/// </summary>
public sealed partial class CatalogProductCreatedHandler(
    ILogger<CatalogProductCreatedHandler> logger)
{
    public async Task HandleAsync(
        CatalogProductCreatedEvent @event,
        CancellationToken cancellationToken)
    {
        LogProductCreatedNotification(@event.ProductId, @event.Name, @event.Price);

        // TODO: Phase 3 — integrate with Granit.Notifications to send email/SignalR notifications
        await Task.CompletedTask;
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Notification triggered for new product {ProductId}: {ProductName} at {Price:C}")]
    private partial void LogProductCreatedNotification(Guid productId, string productName, decimal price);
}
