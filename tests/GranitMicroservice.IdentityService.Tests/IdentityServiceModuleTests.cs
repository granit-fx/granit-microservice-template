using Granit.Modularity;
using Shouldly;

namespace GranitMicroservice.IdentityService.Tests;

public sealed class IdentityServiceModuleTests
{
    [Fact]
    public void Module_should_inherit_from_GranitModule()
    {
        var module = new IdentityServiceModule();

        module.ShouldBeAssignableTo<GranitModule>();
    }

    [Fact]
    public void Module_should_declare_dependencies_via_DependsOn()
    {
        var attributes = typeof(IdentityServiceModule)
            .GetCustomAttributes(typeof(DependsOnAttribute), inherit: false);

        attributes.ShouldNotBeEmpty();
    }
}
