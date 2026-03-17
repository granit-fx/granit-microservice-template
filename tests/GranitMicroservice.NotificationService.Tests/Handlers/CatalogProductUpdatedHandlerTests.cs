using Granit.Notifications.Abstractions;
using GranitMicroservice.NotificationService.Handlers;
using GranitMicroservice.NotificationService.Notifications;
using GranitMicroservice.Shared.Events;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace GranitMicroservice.NotificationService.Tests.Handlers;

public sealed class CatalogProductUpdatedHandlerTests
{
    private readonly INotificationPublisher _publisher =
        Substitute.For<INotificationPublisher>();

    private readonly ILogger<CatalogProductUpdatedHandler> _logger =
        Substitute.For<ILogger<CatalogProductUpdatedHandler>>();

    private readonly CatalogProductUpdatedHandler _handler;

    public CatalogProductUpdatedHandlerTests() =>
        _handler = new CatalogProductUpdatedHandler(_publisher, _logger);

    [Fact]
    public async Task Should_handle_product_updated_event_without_throwing()
    {
        var @event = new CatalogProductUpdatedEvent(
            Guid.NewGuid(), "Updated Product", 49.99m);

        var act = () => _handler.HandleAsync(@event, CancellationToken.None);

        await Should.NotThrowAsync(act);
    }

    [Fact]
    public async Task Should_publish_notification_for_updated_product()
    {
        var @event = new CatalogProductUpdatedEvent(
            Guid.NewGuid(), "Updated Product", 59.99m);

        await _handler.HandleAsync(@event, CancellationToken.None);

        await _publisher.Received(1)
            .PublishToSubscribersAsync(
                Arg.Any<Granit.Notifications.NotificationType<ProductUpdatedData>>(),
                Arg.Is<ProductUpdatedData>(d =>
                    d.ProductId == @event.ProductId &&
                    d.ProductName == @event.Name &&
                    d.Price == @event.Price),
                CancellationToken.None);
    }
}
