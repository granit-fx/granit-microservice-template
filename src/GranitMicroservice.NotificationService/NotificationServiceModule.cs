using Granit.Core.Modularity;
using Granit.Notifications;
using Granit.Persistence;

namespace GranitMicroservice.NotificationService;

[DependsOn(
    typeof(GranitNotificationsModule),
    typeof(GranitPersistenceModule))]
public sealed class NotificationServiceModule : GranitModule;
