using Granit.Core.Modularity;
using Granit.Notifications;
using Granit.Persistence;
using Granit.Persistence.Migrations;

namespace GranitMicroservice.NotificationService;

[DependsOn(
    typeof(GranitNotificationsModule),
    typeof(GranitPersistenceModule),
    typeof(GranitPersistenceMigrationsModule))]
public sealed class NotificationServiceModule : GranitModule;
