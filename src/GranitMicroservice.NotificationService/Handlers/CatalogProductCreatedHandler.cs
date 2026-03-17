using Granit.Notifications.Abstractions;
using GranitMicroservice.NotificationService.Notifications;
using GranitMicroservice.Shared.Events;
using Microsoft.Extensions.Logging;

namespace GranitMicroservice.NotificationService.Handlers;

/// <summary>
/// Handles <see cref="CatalogProductCreatedEvent"/> from the CatalogService
/// and publishes an in-app notification to all subscribers of
/// <see cref="CatalogNotifications.ProductCreated"/>.
/// </summary>
public sealed partial class CatalogProductCreatedHandler(
    INotificationPublisher notificationPublisher,
    ILogger<CatalogProductCreatedHandler> logger)
{
    public async Task HandleAsync(
        CatalogProductCreatedEvent @event,
        CancellationToken cancellationToken)
    {
        LogProductCreated(@event.ProductId, @event.Name, @event.Price);

        // PublishToSubscribersAsync fans out to every user who opted in to
        // "Catalog.ProductCreated" notifications. The in-app channel stores
        // a UserNotification in the notifications database; additional channels
        // (email, SMS, push) can be added to CatalogNotifications.ProductCreated
        // once the corresponding Granit provider packages are configured.
        await notificationPublisher.PublishToSubscribersAsync(
            CatalogNotifications.ProductCreated,
            new ProductCreatedData(@event.ProductId, @event.Name, @event.Price),
            cancellationToken).ConfigureAwait(false);
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Publishing ProductCreated notification for product {ProductId}: {ProductName} at {Price:C}")]
    private partial void LogProductCreated(Guid productId, string productName, decimal price);
}
