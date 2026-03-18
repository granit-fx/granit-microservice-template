using Granit.Bundle.Essentials;
using Granit.Core.Extensions;
using Granit.Persistence.Hosting.Extensions;
using GranitMicroservice.ServiceDefaults;
using Microsoft.AspNetCore.Builder;

namespace GranitMicroservice.Shared.Hosting.Extensions;

public static class SharedHostingExtensions
{
    /// <summary>
    /// Configures cross-cutting concerns: Aspire ServiceDefaults, Granit Essentials,
    /// Wolverine/PostgreSQL outbox, JWT Bearer auth, Redis caching, HTTP resilience,
    /// and the <c>--migrate</c> CLI mode for database migrations.
    /// </summary>
    public static async Task<WebApplicationBuilder> AddSharedHostingAsync(
        this WebApplicationBuilder builder,
        Action<SharedHostingModule>? configureModule = null)
    {
        builder.AddServiceDefaults();

        await builder.AddGranitAsync(granit => granit
            .AddEssentials()
            .AddModule<SharedHostingModule>());

        builder.AddGranitMigrateSupport();

        return builder;
    }
}
