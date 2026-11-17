using Microsoft.Extensions.Options;
using QSR.Application.Interfaces.Integrations;

namespace QSR.Infrastructure.Integrations;

public sealed class IntegrationResolver(
    IEnumerable<IIntegrationProvider> providers,
    IOptions<IntegrationOptions> options) : IIntegrationResolver
{
    public IIntegrationProvider Resolve(IntegrationKind kind)
    {
        if (!options.Value.Providers.TryGetValue(kind.ToString(), out var name) ||
            string.IsNullOrWhiteSpace(name))
            throw new IntegrationUnavailableException(kind);

        return providers.SingleOrDefault(provider =>
            provider.Kind == kind && provider.Name == name)
            ?? throw new IntegrationUnavailableException(kind);
    }

    public IReadOnlyCollection<IntegrationAvailability> GetAvailability()
    {
        return Enum.GetValues<IntegrationKind>().Select(kind =>
        {
            var available = options.Value.Providers.TryGetValue(
                kind.ToString(), out var name) &&
                providers.Count(provider =>
                    provider.Kind == kind && provider.Name == name) == 1;

            return new IntegrationAvailability(kind, available);
        }).ToArray();
    }
}
