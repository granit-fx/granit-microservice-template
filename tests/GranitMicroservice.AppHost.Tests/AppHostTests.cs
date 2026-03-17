using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Shouldly;

namespace GranitMicroservice.AppHost.Tests;

/// <summary>
/// Verifies the Aspire resource model declared in AppHost/Program.cs.
/// These tests run without Docker — no containers are started.
/// </summary>
public sealed class AppHostTests : IAsyncLifetime
{
    private IDistributedApplicationTestingBuilder _appHost = null!;

    public async ValueTask InitializeAsync() =>
        _appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.GranitMicroservice_AppHost>();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    // ── Infrastructure resources ──────────────────────────────────────────────

    [Fact]
    public void Should_declare_postgres_redis_rabbitmq_and_keycloak()
    {
        var names = ResourceNames();

        names.ShouldContain("postgres");
        names.ShouldContain("redis");
        names.ShouldContain("messaging");
        names.ShouldContain("keycloak");
    }

    // ── Databases ─────────────────────────────────────────────────────────────

    [Fact]
    public void Should_declare_three_isolated_databases()
    {
        var names = ResourceNames();

        names.ShouldContain("identity-db");
        names.ShouldContain("catalog-db");
        names.ShouldContain("notification-db");
    }

    [Fact]
    public void Databases_should_be_children_of_postgres_server()
    {
        var postgres = _appHost.Resources
            .OfType<PostgresServerResource>()
            .Single();

        List<string> dbKeys = [.. postgres.Databases.Keys];

        dbKeys.ShouldContain("identity-db");
        dbKeys.ShouldContain("catalog-db");
        dbKeys.ShouldContain("notification-db");
    }

    // ── Service projects ──────────────────────────────────────────────────────

    [Fact]
    public void Should_declare_all_four_service_projects()
    {
        var projectNames = _appHost.Resources
            .OfType<ProjectResource>()
            .Select(r => r.Name)
            .ToList();

        projectNames.ShouldContain("identity-service");
        projectNames.ShouldContain("catalog-service");
        projectNames.ShouldContain("notification-service");
        projectNames.ShouldContain("api-gateway");
    }

    // ── WaitFor dependencies (startup ordering) ───────────────────────────────

    [Fact]
    public void Catalog_service_should_wait_for_catalog_db_and_messaging()
    {
        var waits = WaitTargets("catalog-service");

        waits.ShouldContain("catalog-db");
        waits.ShouldContain("messaging");
    }

    [Fact]
    public void Identity_service_should_wait_for_identity_db_messaging_and_keycloak()
    {
        var waits = WaitTargets("identity-service");

        waits.ShouldContain("identity-db");
        waits.ShouldContain("messaging");
        waits.ShouldContain("keycloak");
    }

    [Fact]
    public void Notification_service_should_wait_for_notification_db_and_messaging()
    {
        var waits = WaitTargets("notification-service");

        waits.ShouldContain("notification-db");
        waits.ShouldContain("messaging");
    }

    [Fact]
    public void Api_gateway_should_wait_for_identity_service_catalog_service_and_keycloak()
    {
        var waits = WaitTargets("api-gateway");

        waits.ShouldContain("identity-service");
        waits.ShouldContain("catalog-service");
        waits.ShouldContain("keycloak");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private List<string> ResourceNames() =>
        _appHost.Resources.Select(r => r.Name).ToList();

    private List<string> WaitTargets(string resourceName)
    {
        var resource = _appHost.Resources.Single(r => r.Name == resourceName);
        return resource.Annotations
            .OfType<WaitAnnotation>()
            .Select(a => a.Resource.Name)
            .ToList();
    }
}
