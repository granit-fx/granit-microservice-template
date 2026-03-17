using Granit.Core.Modularity;
using Granit.Persistence;
using Granit.Persistence.DataSeeding;
using Granit.Persistence.Extensions;
using Granit.RateLimiting;
using Granit.Validation;
using Granit.Validation.Extensions;
using GranitMicroservice.CatalogService.Persistence;

namespace GranitMicroservice.CatalogService;

[DependsOn(
    typeof(GranitPersistenceModule),
    typeof(GranitRateLimitingModule),
    typeof(GranitValidationModule))]
public sealed class CatalogServiceModule : GranitModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddGranitValidatorsFromAssemblyContaining<CatalogServiceModule>();
        context.Services.AddGranitDataSeeding();
        context.Services.AddTransient<IDataSeedContributor, CatalogDataSeedContributor>();
    }
}
