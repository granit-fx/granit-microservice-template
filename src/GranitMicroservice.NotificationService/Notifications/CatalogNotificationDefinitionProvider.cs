using Granit.Notifications;
using Granit.Notifications.Abstractions;

namespace GranitMicroservice.NotificationService.Notifications;

/// <summary>
/// Registers catalog notification types at startup so the framework can populate
/// the definition store (used for user preference management and channel resolution).
/// </summary>
internal sealed class CatalogNotificationDefinitionProvider : INotificationDefinitionProvider
{
    public void Define(INotificationDefinitionContext context)
    {
        context.Add(new NotificationDefinition(CatalogNotifications.ProductCreated.Name)
        {
            DisplayName = "New product added",
            Description = "Sent when a new product is published to the catalog.",
            GroupName = "Catalog",
            DefaultChannels = [NotificationChannels.InApp],
            AllowUserOptOut = true,
        });

        context.Add(new NotificationDefinition(CatalogNotifications.ProductUpdated.Name)
        {
            DisplayName = "Product updated",
            Description = "Sent when an existing product's details or price change.",
            GroupName = "Catalog",
            DefaultChannels = [NotificationChannels.InApp],
            AllowUserOptOut = true,
        });
    }
}
