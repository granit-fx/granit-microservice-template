using Granit.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GranitMicroservice.IdentityService.Persistence;

/// <summary>
/// Design-time factory for EF Core migrations tooling (<c>dotnet ef migrations add</c>).
/// Not used at runtime — the DI-configured <see cref="IdentityServiceDbContext"/> is used instead.
/// </summary>
internal sealed class IdentityServiceDbContextFactory : IDesignTimeDbContextFactory<IdentityServiceDbContext>
{
    public IdentityServiceDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = configuration.GetConnectionString("identity-db")
            ?? throw new InvalidOperationException(
                "Connection string 'identity-db' not found. " +
                "Set it in appsettings.Development.json or via the ConnectionStrings__identity-db environment variable.");

        DbContextOptionsBuilder<IdentityServiceDbContext> builder = new();
        builder.UseNpgsql(connectionString);
        return new IdentityServiceDbContext(builder.Options, GranitDesignTime.CurrentTenant, GranitDesignTime.DataFilter);
    }
}
