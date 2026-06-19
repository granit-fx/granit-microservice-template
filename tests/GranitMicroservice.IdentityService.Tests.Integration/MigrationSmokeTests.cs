using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace GranitMicroservice.IdentityService.Tests.Integration;

/// <summary>
/// IdentityServiceDbContext folds three EF stores — the canonical Identity user
/// aggregate, the federated identity cache, and the audit trail — into a single
/// migrated context (see IdentityServiceDbContext.OnGranitModelCreating). These
/// tests prove that one schema build covers all three on a real PostgreSQL, so the
/// single migration genuinely owns every folded table.
/// </summary>
public sealed class MigrationSmokeTests(IdentityDbFixture fixture) : IClassFixture<IdentityDbFixture>
{
    [Fact]
    public async Task Should_apply_schema_successfully()
    {
        await using var db = fixture.CreateDbContext();

        var canConnect = await db.Database.CanConnectAsync();

        canConnect.ShouldBeTrue();
    }

    [Theory]
    [InlineData("identity_users")]                          // ConfigureGranitIdentityModule — canonical User aggregate
    [InlineData("identity_federated_user_cache_entries")]   // ConfigureIdentityModule — federated identity cache
    [InlineData("audit_log_log_entries")]                   // ConfigureAuditingModule — audit trail
    public async Task Should_have_folded_table(string tableName)
    {
        await using var db = fixture.CreateDbContext();

        var tables = await db.Database
            .SqlQueryRaw<string>(
                "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'")
            .ToListAsync();

        tables.ShouldContain(tableName);
    }
}
