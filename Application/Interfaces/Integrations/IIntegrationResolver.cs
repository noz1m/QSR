namespace QSR.Application.Interfaces.Integrations;

public interface IIntegrationResolver
{
    IIntegrationProvider Resolve(IntegrationKind kind);
    IReadOnlyCollection<IntegrationAvailability> GetAvailability();
}

public sealed record IntegrationAvailability(
    IntegrationKind Kind,
    bool Available);
