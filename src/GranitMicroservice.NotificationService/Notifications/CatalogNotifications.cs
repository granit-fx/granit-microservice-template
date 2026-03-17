using Granit.Notifications;

namespace GranitMicroservice.NotificationService.Notifications;

/// <summary>
/// Catalog notification type singletons.
/// Each type declares its name (used as a stable identifier across deploys)
/// and the default delivery channels.
/// </summary>
public static class CatalogNotifications
{
    /// <summary>
    /// Fired when a new product is added to the catalog.
    /// Delivered in-app to all subscribers (e.g. buyers, catalog managers).
    /// </summary>
    public static readonly ProductCreatedNotification ProductCreated = new();

    /// <summary>
    /// Fired when an existing product is updated.
    /// Delivered in-app to all subscribers.
    /// </summary>
    public static readonly ProductUpdatedNotification ProductUpdated = new();

    public sealed class ProductCreatedNotification : NotificationType<ProductCreatedData>
    {
        public override string Name => "Catalog.ProductCreated";
        public override NotificationSeverity DefaultSeverity => NotificationSeverity.Info;
        public override IReadOnlyList<string> DefaultChannels => [NotificationChannels.InApp];
    }

    public sealed class ProductUpdatedNotification : NotificationType<ProductUpdatedData>
    {
        public override string Name => "Catalog.ProductUpdated";
        public override NotificationSeverity DefaultSeverity => NotificationSeverity.Info;
        public override IReadOnlyList<string> DefaultChannels => [NotificationChannels.InApp];
    }
}
