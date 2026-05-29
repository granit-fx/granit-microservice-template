using Granit.Modularity;
using Granit.Notifications;
using Granit.Notifications.Email.Extensions;
using Granit.Notifications.Email.Smtp.Extensions;
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
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddNotificationDefinitions<CatalogNotificationDefinitionProvider>();

        // Email channel via SMTP — wired to the Mailpit container in AppHost for
        // local dev. To actually deliver, opt-in by adding NotificationChannels.Email
        // to a notification's DefaultChannels and register an IRecipientResolver.
        context.Services.AddGranitNotificationsEmail();
        context.Services.AddGranitNotificationsEmailSmtp();
    }
}
