using SecretManager.SecretsDashboard.Domain;

namespace SecretManager.SecretsDashboard.Domain.Tests;

public class AssemblyMarkerTests
{
    [Fact]
    public void DomainAssemblyMarker_is_available()
    {
        Assert.NotNull(typeof(AssemblyMarker));
    }
}
