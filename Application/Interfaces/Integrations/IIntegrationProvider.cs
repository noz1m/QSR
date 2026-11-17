namespace QSR.Application.Interfaces.Integrations;

public interface IIntegrationProvider
{
    string Name { get; }
    IntegrationKind Kind { get; }
}

public enum IntegrationKind
{
    Payments,
    Sms,
    Delivery,
    Fiscalization
}
