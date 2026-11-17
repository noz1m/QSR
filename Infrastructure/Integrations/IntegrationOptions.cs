namespace QSR.Infrastructure.Integrations;

public sealed class IntegrationOptions
{
    public Dictionary<string, string> Providers { get; set; } = new();
}
