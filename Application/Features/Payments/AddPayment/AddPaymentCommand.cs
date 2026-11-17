using QSR.Domain.Enums;

namespace QSR.Application.Features.Payments.AddPayment;

public sealed record AddPaymentCommand(
    Guid OrderId,
    PaymentMethod Method,
    decimal Amount,
    string Currency,
    string? ExternalTransactionId);