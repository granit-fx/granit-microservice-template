using Granit.Core.DataFiltering;
using Granit.Core.MultiTenancy;
using Granit.Notifications.EntityFrameworkCore.Extensions;
using Granit.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GranitMicroservice.NotificationService.Persistence;

public sealed class NotificationServiceDbContext(
    DbContextOptions<NotificationServiceDbContext> options,
    ICurrentTenant? currentTenant = null,
    IDataFilter? dataFilter = null) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureNotificationsModule();
        modelBuilder.ApplyGranitConventions(currentTenant, dataFilter);
    }
}
