namespace QSR.Application.Features.Payments.RefundPayment;

public sealed record RefundPaymentCommand(
    Guid PaymentId,
    decimal Amount,
    string? Reason,
    string? ExternalTransactionId);