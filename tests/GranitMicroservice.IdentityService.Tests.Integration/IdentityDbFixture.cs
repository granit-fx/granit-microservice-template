using Granit.Persistence.EntityFrameworkCore;
using GranitMicroservice.IdentityService.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace GranitMicroservice.IdentityService.Tests.Integration;

public sealed class IdentityDbFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    public IdentityServiceDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityServiceDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        return new IdentityServiceDbContext(options, GranitDesignTime.CurrentTenant, GranitDesignTime.DataFilter);
    }

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        await using var db = CreateDbContext();
        await db.Database.EnsureCreatedAsync();
    }

    public async ValueTask DisposeAsync() =>
        await _container.DisposeAsync();
}
