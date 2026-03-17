using Granit.Core.Modularity;
using Granit.Persistence;
using Granit.Validation;
using Granit.Validation.Extensions;

namespace GranitMicroservice.CatalogService;

[DependsOn(
    typeof(GranitPersistenceModule),
    typeof(GranitValidationModule))]
public sealed class CatalogServiceModule : GranitModule
{
    public override void ConfigureServices(ServiceConfigurationContext context) =>
        context.Services.AddGranitValidatorsFromAssemblyContaining<CatalogServiceModule>();
}
