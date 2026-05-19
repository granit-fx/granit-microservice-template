using Granit.DataFiltering;
using Granit.MultiTenancy;
using Granit.Notifications.EntityFrameworkCore.Extensions;
using Granit.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GranitMicroservice.NotificationService.Persistence;

public sealed class NotificationServiceDbContext(
    DbContextOptions<NotificationServiceDbContext> options,
    ICurrentTenant currentTenant,
    IDataFilter? dataFilter = null) : GranitDbContext(options, currentTenant, dataFilter)
{
    protected override void OnGranitModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureNotificationsModule();
    }
}
