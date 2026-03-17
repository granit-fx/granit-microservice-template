using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace GranitMicroservice.AppHost.Tests;

public sealed class AppHostTests
{
    [Fact]
    public async Task AppHost_should_build_without_errors()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.GranitMicroservice_AppHost>();

        await using var app = await builder.BuildAsync();

        app.ShouldNotBeNull();
    }

    [Fact]
    public async Task AppHost_should_expose_catalog_service_endpoint()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.GranitMicroservice_AppHost>();

        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        var client = app.CreateHttpClient("catalog-service");
        client.ShouldNotBeNull();
    }

    [Fact]
    public async Task AppHost_should_expose_identity_service_endpoint()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.GranitMicroservice_AppHost>();

        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        var client = app.CreateHttpClient("identity-service");
        client.ShouldNotBeNull();
    }

    [Fact]
    public async Task AppHost_should_expose_api_gateway_endpoint()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.GranitMicroservice_AppHost>();

        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        var client = app.CreateHttpClient("api-gateway");
        client.ShouldNotBeNull();
    }
}
