using Granit.DataFiltering;
using Granit.MultiTenancy;
using Granit.Notifications.EntityFrameworkCore.Extensions;
using Granit.Persistence.EntityFrameworkCore.Extensions;
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
