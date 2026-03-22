using Granit.Core.Modularity;
using Granit.Notifications;
using Granit.Notifications.Extensions;
using Granit.Persistence;
using Granit.Persistence.Hosting;
using GranitMicroservice.NotificationService.Notifications;
using GranitMicroservice.NotificationService.Persistence;

namespace GranitMicroservice.NotificationService;

[DependsOn(
    typeof(GranitNotificationsModule),
    typeof(GranitPersistenceModule))]
public sealed class NotificationServiceModule : GranitModule, IMigratableModule<NotificationServiceDbContext>
{
    public override void ConfigureServices(ServiceConfigurationContext context) =>
        context.Services.AddNotificationDefinitions<CatalogNotificationDefinitionProvider>();
}
