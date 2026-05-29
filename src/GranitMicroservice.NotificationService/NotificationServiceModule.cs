using Granit.Modularity;
using Granit.Notifications;
using Granit.Notifications.Abstractions;
using Granit.Notifications.EntityFrameworkCore;
using Granit.Notifications.EntityFrameworkCore.Extensions;
using Granit.Notifications.Email.Extensions;
using Granit.Notifications.Email.Smtp.Extensions;
using Granit.Notifications.Extensions;
using Granit.Notifications.MobilePush.Extensions;
using Granit.Persistence.EntityFrameworkCore;
using Granit.Persistence.EntityFrameworkCore.Hosting;
using Granit.Persistence.MultiTenancy;
using GranitMicroservice.NotificationService.Notifications;
using GranitMicroservice.NotificationService.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GranitMicroservice.NotificationService;

[DependsOn(
    typeof(GranitNotificationsModule),
    typeof(GranitNotificationsEntityFrameworkCoreModule),
    typeof(GranitPersistenceEntityFrameworkCoreModule))]
public sealed class NotificationServiceModule : GranitModule, IMigratableModule<NotificationServiceDbContext>
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddNotificationDefinitions<CatalogNotificationDefinitionProvider>();

        // Email channel via SMTP — wired to the Mailpit container in AppHost for
        // local dev. To actually deliver, opt-in by adding NotificationChannels.Email
        // to a notification's DefaultChannels (catalog notifications are InApp-only
        // by default).
        context.Services.AddGranitNotificationsEmail();
        context.Services.AddGranitNotificationsEmailSmtp();

        // Durable EF Core stores for user notifications + preferences (replaces the
        // default in-memory stores). Shared mode keeps every tenant's rows in one host
        // table with a row-level filter — the behavioural default. Migrations for the
        // table are owned by NotificationServiceDbContext via ConfigureNotificationsModule.
        context.Builder.AddGranitNotificationsEntityFrameworkCore(opts =>
        {
            opts.StorageMode = DualScopeStorageMode.Shared;
            opts.Configure = db => db.UseNpgsql(
                context.Builder.Configuration.GetConnectionString("notification-db"));

            // Pour activer l'isolation physique par tenant (RGPD Art. 17, ISO 27001 A.8.12):
            // opts.StorageMode = DualScopeStorageMode.Segregated;
            // opts.ConfigureHost = db => db.UseNpgsql(hostConnString);
            // opts.ConfigureSchemaPerTenant = db => db.UseNpgsql(baseConnString);
        });

        // AddGranitNotificationsEntityFrameworkCore unconditionally registers
        // EfCoreMobilePushTokenStore, which depends on IMobilePushTokenHasher — provided
        // by AddGranitNotificationsMobilePush (HMAC over a configured device-token pepper).
        // The package's in-memory token store is replaced by the EF Core one above; this
        // call only supplies the hasher + options binding, no mobile-push channel is used.
        context.Services.AddGranitNotificationsMobilePush();

        // Every Granit notification channel resolves recipient contact info through an
        // IRecipientResolver. Granit ships no default — recipient lookup is
        // application-specific — so the host must register one (see
        // NotificationRecipientResolver for the contract and a template placeholder).
        context.Services.TryAddScoped<IRecipientResolver, NotificationRecipientResolver>();
    }
}
