using Granit.Core.Modularity;
using Granit.Notifications;
using Granit.Notifications.Extensions;
using Granit.Persistence;
using GranitMicroservice.NotificationService.Notifications;

namespace GranitMicroservice.NotificationService;

[DependsOn(
    typeof(GranitNotificationsModule),
    typeof(GranitPersistenceModule))]
public sealed class NotificationServiceModule : GranitModule
{
    public override void ConfigureServices(ServiceConfigurationContext context) =>
        context.Services.AddNotificationDefinitions<CatalogNotificationDefinitionProvider>();
}
