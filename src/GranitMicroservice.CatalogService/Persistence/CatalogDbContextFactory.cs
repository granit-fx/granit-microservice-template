using Granit.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GranitMicroservice.CatalogService.Persistence;

/// <summary>
/// Design-time factory for EF Core migrations tooling (<c>dotnet ef migrations add</c>).
/// Not used at runtime — the DI-configured <see cref="CatalogDbContext"/> is used instead.
/// </summary>
internal sealed class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = configuration.GetConnectionString("catalog-db")
            ?? throw new InvalidOperationException(
                "Connection string 'catalog-db' not found. " +
                "Set it in appsettings.Development.json or via the ConnectionStrings__catalog-db environment variable.");

        DbContextOptionsBuilder<CatalogDbContext> builder = new();
        builder.UseNpgsql(connectionString);
        return new CatalogDbContext(builder.Options, GranitDesignTime.CurrentTenant, GranitDesignTime.DataFilter);
    }
}
