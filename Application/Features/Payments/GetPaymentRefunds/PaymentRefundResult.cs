using QSR.Domain.Enums;

namespace QSR.Application.Features.Payments.GetPaymentRefunds;

public sealed record PaymentRefundResult(
    Guid Id,
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    string Currency,
    PaymentRefundStatus Status,
    string? Reason,
    string? ExternalTransactionId,
    DateTimeOffset CreatedAt);