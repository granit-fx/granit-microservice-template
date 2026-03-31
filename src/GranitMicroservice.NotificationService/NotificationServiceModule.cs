using Granit.Modularity;
using Granit.Notifications;
using Granit.Notifications.Extensions;
using Granit.Persistence.EntityFrameworkCore;
using Granit.Persistence.EntityFrameworkCore.Hosting;
using GranitMicroservice.NotificationService.Notifications;
using GranitMicroservice.NotificationService.Persistence;

namespace GranitMicroservice.NotificationService;

[DependsOn(
    typeof(GranitNotificationsModule),
    typeof(GranitPersistenceEntityFrameworkCoreModule))]
public sealed class NotificationServiceModule : GranitModule, IMigratableModule<NotificationServiceDbContext>
{
    public override void ConfigureServices(ServiceConfigurationContext context) =>
        context.Services.AddNotificationDefinitions<CatalogNotificationDefinitionProvider>();
}
