namespace QSR.Application.Interfaces.Integrations;

public sealed class IntegrationUnavailableException(IntegrationKind kind)
    : Exception($"Integration {kind} is not configured.");
