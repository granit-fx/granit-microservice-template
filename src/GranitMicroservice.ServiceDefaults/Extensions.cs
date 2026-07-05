using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using RabbitMQ.Client;

namespace GranitMicroservice.ServiceDefaults;

/// <summary>
/// Shared service defaults bridging .NET Aspire conventions with Granit modules.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Configures OpenTelemetry, health checks, resilient HTTP clients, and service discovery.
    /// </summary>
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetry();
        builder.AddDefaultHealthChecks();
        builder.Services.AddServiceDiscovery();
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });

        return builder;
    }

    /// <summary>
    /// Maps health check endpoints: /health/live, /health/ready, /health/startup.
    /// </summary>
    /// <remarks>
    /// Accepts both <c>"ready"</c> and <c>"readiness"</c> tags on the readiness probe so that
    /// Granit framework health checks (which use <c>"readiness"</c>) are included automatically.
    /// </remarks>
    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live"),
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            // Accept both "ready" (our convention) and "readiness" (Granit.Persistence.EntityFrameworkCore convention).
            Predicate = r => r.Tags.Contains("ready") || r.Tags.Contains("readiness"),
        });

        app.MapHealthChecks("/health/startup", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("startup"),
        });

        return app;
    }

    /// <summary>
    /// Adds a RabbitMQ readiness/startup health check using the <c>"messaging"</c> Aspire connection.
    /// No-ops when the connection string is absent (e.g. unit tests).
    /// </summary>
    public static IHostApplicationBuilder AddRabbitMqHealthCheck(this IHostApplicationBuilder builder)
    {
        string? connectionString = builder.Configuration.GetConnectionString("messaging");
        if (connectionString is null)
        {
            return builder;
        }

        builder.Services.AddHealthChecks()
            .AddRabbitMQ(
                async _ =>
                {
                    // RabbitMQ.Client v7: create an async connection from the AMQP URI.
                    // Aspire injects the full URI (amqp://user:pass@host:port/) via WithReference().
                    // The "health-check" name appears in the RabbitMQ management UI connection list.
                    var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
                    return await factory.CreateConnectionAsync("health-check");
                },
                name: "rabbitmq",
                tags: ["ready", "startup"]);

        return builder;
    }

    private static void ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        // OTLP export (UseOtlpExporter) is owned by GranitObservabilityModule, loaded via
        // Granit.Bundle.Essentials in SharedHostingModule and explicitly in ApiGatewayModule.
        // OpenTelemetry SDK 1.9+ forbids multiple UseOtlpExporter() calls on the same
        // IServiceCollection — registering it here would crash startup under Aspire.
    }

    private static void AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live", "ready", "startup"]);
    }
}
