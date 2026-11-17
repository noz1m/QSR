using QSR.Domain.Enums;

namespace QSR.Application.Features.Payments.AddPayment;

public sealed record AddPaymentResult(
    Guid PaymentId,
    Guid OrderId,
    PaymentMethod Method,
    PaymentStatus PaymentStatus,
    decimal Amount,
    string Currency,
    OrderStatus OrderStatus);