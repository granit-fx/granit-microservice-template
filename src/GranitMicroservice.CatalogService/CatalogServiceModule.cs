using Granit.Http.ApiDocumentation;
using Granit.Modularity;
using Granit.Persistence.EntityFrameworkCore;
using Granit.Persistence.EntityFrameworkCore.DataSeeding;
using Granit.Persistence.EntityFrameworkCore.Extensions;
using Granit.Persistence.EntityFrameworkCore.Hosting;
using Granit.RateLimiting;
using Granit.Validation;
using Granit.Validation.Extensions;
using GranitMicroservice.CatalogService.Persistence;

namespace GranitMicroservice.CatalogService;

[DependsOn(
    typeof(GranitHttpApiDocumentationModule),
    typeof(GranitPersistenceEntityFrameworkCoreModule),
    typeof(GranitRateLimitingModule),
    typeof(GranitValidationModule))]
public sealed class CatalogServiceModule : GranitModule, IMigratableModule<CatalogDbContext>
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddGranitValidatorsFromAssemblyContaining<CatalogServiceModule>();
        context.Services.AddGranitDataSeeding();
        context.Services.AddTransient<IDataSeedContributor, CatalogDataSeedContributor>();
    }
}
