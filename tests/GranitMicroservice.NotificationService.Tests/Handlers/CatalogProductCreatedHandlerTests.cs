using GranitMicroservice.NotificationService.Handlers;
using GranitMicroservice.Shared.Events;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace GranitMicroservice.NotificationService.Tests.Handlers;

public sealed class CatalogProductCreatedHandlerTests
{
    private readonly ILogger<CatalogProductCreatedHandler> _logger =
        Substitute.For<ILogger<CatalogProductCreatedHandler>>();

    private readonly CatalogProductCreatedHandler _handler;

    public CatalogProductCreatedHandlerTests() =>
        _handler = new CatalogProductCreatedHandler(_logger);

    [Fact]
    public async Task Should_handle_product_created_event_without_throwing()
    {
        var @event = new CatalogProductCreatedEvent(
            Guid.NewGuid(), "Test Product", 29.99m);

        var act = () => _handler.HandleAsync(@event, CancellationToken.None);

        await Should.NotThrowAsync(act);
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
