using Granit.Notifications.Abstractions;
using GranitMicroservice.NotificationService.Handlers;
using GranitMicroservice.Shared.Events;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace GranitMicroservice.NotificationService.Tests.Handlers;

public sealed class CatalogProductCreatedHandlerTests
{
    private readonly INotificationPublisher _publisher =
        Substitute.For<INotificationPublisher>();

    private readonly ILogger<CatalogProductCreatedHandler> _logger =
        Substitute.For<ILogger<CatalogProductCreatedHandler>>();

    private readonly CatalogProductCreatedHandler _handler;

    public CatalogProductCreatedHandlerTests() =>
        _handler = new CatalogProductCreatedHandler(_publisher, _logger);

    [Fact]
    public async Task Should_handle_product_created_event_without_throwing()
    {
        var @event = new CatalogProductCreatedEvent(
            Guid.NewGuid(), "Test Product", 29.99m);

        var act = () => _handler.HandleAsync(@event, CancellationToken.None);

        await Should.NotThrowAsync(act);
    }

    [Fact]
    public async Task Should_publish_notification_for_created_product()
    {
        var @event = new CatalogProductCreatedEvent(
            Guid.NewGuid(), "New Product", 49.99m);

        await _handler.HandleAsync(@event, CancellationToken.None);

        await _publisher.Received(1)
            .PublishToSubscribersAsync(
                Arg.Any<Granit.Notifications.NotificationType<Notifications.ProductCreatedData>>(),
                Arg.Is<Notifications.ProductCreatedData>(d =>
                    d.ProductId == @event.ProductId &&
                    d.ProductName == @event.Name &&
                    d.Price == @event.Price),
                CancellationToken.None);
    }

    [Fact]
    public async Task Should_handle_zero_price_product()
    {
        var @event = new CatalogProductCreatedEvent(
            Guid.NewGuid(), "Free Product", 0m);

        var act = () => _handler.HandleAsync(@event, CancellationToken.None);

        await Should.NotThrowAsync(act);
    }
}
