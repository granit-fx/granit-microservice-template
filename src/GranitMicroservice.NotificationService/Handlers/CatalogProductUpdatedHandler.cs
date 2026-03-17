using Granit.Notifications.Abstractions;
using GranitMicroservice.NotificationService.Notifications;
using GranitMicroservice.Shared.Events;
using Microsoft.Extensions.Logging;

namespace GranitMicroservice.NotificationService.Handlers;

/// <summary>
/// Handles <see cref="CatalogProductUpdatedEvent"/> from the CatalogService
/// and publishes an in-app notification to all subscribers of
/// <see cref="CatalogNotifications.ProductUpdated"/>.
/// </summary>
public sealed partial class CatalogProductUpdatedHandler(
    INotificationPublisher notificationPublisher,
    ILogger<CatalogProductUpdatedHandler> logger)
{
    public async Task HandleAsync(
        CatalogProductUpdatedEvent @event,
        CancellationToken cancellationToken)
    {
        LogProductUpdated(@event.ProductId, @event.Name, @event.Price);

        await notificationPublisher.PublishToSubscribersAsync(
            CatalogNotifications.ProductUpdated,
            new ProductUpdatedData(@event.ProductId, @event.Name, @event.Price),
            cancellationToken).ConfigureAwait(false);
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Publishing ProductUpdated notification for product {ProductId}: {ProductName} at {Price:C}")]
    private partial void LogProductUpdated(Guid productId, string productName, decimal price);
}
