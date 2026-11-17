using QSR.Domain.Common;
using QSR.Domain.Enums;
using QSR.Domain.ValueObjects;

namespace QSR.Domain.Entities.Branches.Payments;

public class PaymentRefund : AuditableEntity
{
    private PaymentRefund()
    {
    }

    public Guid PaymentId { get; private set; }

    public Guid OrderId { get; private set; }

    public Money Amount { get; private set; }

    public PaymentRefundStatus Status { get; private set; }

    public string? Reason { get; private set; }

    public string? ExternalTransactionId { get; private set; }

    public static PaymentRefund Create(
        Guid paymentId,
        Guid orderId,
        Money amount,
        string? reason = null)
    {
        if (paymentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Payment ID is required.");
        }

        if (orderId == Guid.Empty)
        {
            throw new ArgumentException(
                "Order ID is required.");
        }

        if (amount.Amount <= 0)
        {
            throw new ArgumentException(
                "Refund amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(amount.Currency))
        {
            throw new ArgumentException(
                "Refund currency is required.");
        }

        return new PaymentRefund
        {
            PaymentId = paymentId,
            OrderId = orderId,
            Amount = amount,
            Reason = string.IsNullOrWhiteSpace(reason)
                ? null
                : reason.Trim(),
            Status = PaymentRefundStatus.Pending
        };
    }

    public void Complete(
        string? externalTransactionId = null)
    {
        if (Status != PaymentRefundStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending refund can be completed.");
        }

        Status = PaymentRefundStatus.Completed;

        ExternalTransactionId =
            string.IsNullOrWhiteSpace(externalTransactionId)
                ? null
                : externalTransactionId.Trim();
    }

    public void Fail()
    {
        if (Status != PaymentRefundStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending refund can be failed.");
        }

        Status = PaymentRefundStatus.Failed;
    }

    public void Cancel()
    {
        if (Status != PaymentRefundStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending refund can be cancelled.");
        }

        Status = PaymentRefundStatus.Cancelled;
    }
}