using QSR.Domain.Enums;

namespace QSR.Application.Features.Payments.GetPaymentsByOrder;

public sealed record GetPaymentResult(
    Guid Id,
    Guid OrderId,
    PaymentMethod Method,
    PaymentStatus Status,
    decimal Amount,
    string Currency,
    string? ExternalTransactionId,
    DateTimeOffset CreatedAt);