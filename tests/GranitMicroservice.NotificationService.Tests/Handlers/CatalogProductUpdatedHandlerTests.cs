using GranitMicroservice.NotificationService.Handlers;
using GranitMicroservice.Shared.Events;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace GranitMicroservice.NotificationService.Tests.Handlers;

public sealed class CatalogProductUpdatedHandlerTests
{
    private readonly ILogger<CatalogProductUpdatedHandler> _logger =
        Substitute.For<ILogger<CatalogProductUpdatedHandler>>();

    private readonly CatalogProductUpdatedHandler _handler;

    public CatalogProductUpdatedHandlerTests() =>
        _handler = new CatalogProductUpdatedHandler(_logger);

    [Fact]
    public async Task Should_handle_product_updated_event()
    {
        var @event = new CatalogProductUpdatedEvent(
            Guid.NewGuid(), "Updated Product", 49.99m);

        var act = () => _handler.HandleAsync(@event, CancellationToken.None);

        await Should.NotThrowAsync(act);
    }
}
