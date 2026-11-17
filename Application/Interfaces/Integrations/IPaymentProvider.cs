namespace QSR.Application.Interfaces.Integrations;

public interface IPaymentProvider : IIntegrationProvider
{
    Task<ProviderPaymentResult> CreateAsync(
        ProviderPaymentRequest request, CancellationToken cancellationToken = default);

    Task<ProviderPaymentResult> GetStatusAsync(
        string externalId, CancellationToken cancellationToken = default);

    Task<ProviderPaymentResult> RefundAsync(
        ProviderRefundRequest request, CancellationToken cancellationToken = default);
}

public sealed record ProviderPaymentRequest(
    Guid OrderId, string IdempotencyKey, decimal Amount, string Currency);

public sealed record ProviderRefundRequest(
    string ExternalPaymentId, string IdempotencyKey, decimal Amount, string Currency);

public sealed record ProviderPaymentResult(
    string ExternalId, ProviderPaymentStatus Status, decimal Amount, string Currency);

public enum ProviderPaymentStatus
{
    Pending,
    Succeeded,
    Failed,
    Cancelled,
    Unknown
}
