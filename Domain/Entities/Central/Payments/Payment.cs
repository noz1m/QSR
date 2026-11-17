using QSR.Domain.Common;
using QSR.Domain.Enums;
using QSR.Domain.ValueObjects;

namespace QSR.Domain.Entities.Branches.Payments;

public class Payment : AuditableEntity
{
    public Guid OrderId { get; private set; }

    public PaymentMethod Method { get; private set; }

    public PaymentStatus Status { get; private set; }

    public Money Amount { get; private set; }

    public string? ExternalTransactionId { get; private set; }

    private Payment()
    {
    }

    public static Payment Create(
        Guid orderId,
        PaymentMethod method,
        Money amount)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException(
                "Order ID is required.");

        if (amount.Amount <= 0)
            throw new ArgumentException(
                "Payment amount must be greater than zero.");

        return new Payment
        {
            OrderId = orderId,
            Method = method,
            Amount = amount,
            Status = PaymentStatus.Pending
        };
    }

    public void Complete(
        string? externalTransactionId = null)
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending payment can be completed.");
        }

        Status = PaymentStatus.Completed;
        ExternalTransactionId = externalTransactionId;
    }

    public void Fail()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending payment can be failed.");
        }

        Status = PaymentStatus.Failed;
    }

    public void Cancel()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending payment can be cancelled.");
        }

        Status = PaymentStatus.Cancelled;
    }

    public void MarkRefunded()
    {
        if (Status != PaymentStatus.Completed)
        {
            throw new InvalidOperationException(
                "Only completed payment can be refunded.");
        }

        Status = PaymentStatus.Refunded;
    }
}