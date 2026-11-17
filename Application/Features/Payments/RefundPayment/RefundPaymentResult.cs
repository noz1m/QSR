using QSR.Domain.Enums;

namespace QSR.Application.Features.Payments.RefundPayment;

public sealed record RefundPaymentResult(
    Guid RefundId,
    Guid PaymentId,
    Guid OrderId,
    decimal RefundedAmount,
    decimal PaymentAmount,
    decimal TotalRefundedAmount,
    decimal RemainingRefundableAmount,
    string Currency,
    PaymentRefundStatus RefundStatus,
    PaymentStatus PaymentStatus,
    OrderStatus OrderStatus);