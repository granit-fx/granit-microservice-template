using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GranitMicroservice.NotificationService.Persistence;

/// <summary>
/// Design-time factory for EF Core migrations tooling (<c>dotnet ef migrations add</c>).
/// Not used at runtime — the DI-configured <see cref="NotificationServiceDbContext"/> is used instead.
/// </summary>
internal sealed class NotificationServiceDbContextFactory : IDesignTimeDbContextFactory<NotificationServiceDbContext>
{
    public NotificationServiceDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = configuration.GetConnectionString("notification-db")
            ?? throw new InvalidOperationException(
                "Connection string 'notification-db' not found. " +
                "Set it in appsettings.Development.json or via the ConnectionStrings__notification-db environment variable.");

        DbContextOptionsBuilder<NotificationServiceDbContext> builder = new();
        builder.UseNpgsql(connectionString);
        return new NotificationServiceDbContext(builder.Options);
    }
}
