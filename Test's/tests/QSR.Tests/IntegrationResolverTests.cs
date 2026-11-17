using Microsoft.Extensions.Options;
using QSR.Application.Interfaces.Integrations;
using QSR.Infrastructure.Integrations;

namespace QSR.Tests;

public class IntegrationResolverTests
{
    [Fact]
    public void UnconfiguredIntegrationsAreUnavailable()
    {
        var resolver = new IntegrationResolver([], Options.Create(new IntegrationOptions()));

        Assert.All(resolver.GetAvailability(), item => Assert.False(item.Available));
        Assert.Throws<IntegrationUnavailableException>(
            () => resolver.Resolve(IntegrationKind.Payments));
    }

    [Fact]
    public void SelectedProviderMustMatchBothNameAndKind()
    {
        var provider = new TestProvider();
        var options = new IntegrationOptions();
        options.Providers["Sms"] = provider.Name;
        options.Providers["Payments"] = provider.Name;
        var resolver = new IntegrationResolver([provider], Options.Create(options));

        Assert.Same(provider, resolver.Resolve(IntegrationKind.Sms));
        Assert.Throws<IntegrationUnavailableException>(
            () => resolver.Resolve(IntegrationKind.Payments));
    }

    private sealed class TestProvider : IIntegrationProvider
    {
        public string Name => "test";
        public IntegrationKind Kind => IntegrationKind.Sms;
    }
}
