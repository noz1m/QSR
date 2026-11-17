using QSR.Domain.Common;
using QSR.Domain.Entities.Branches.Payments;
using QSR.Domain.Enums;
using QSR.Domain.ValueObjects;

namespace QSR.Domain.Entities.Branches.Orders;

public class Order : AuditableEntity
{
    private readonly List<OrderLine> _lines = [];
    private readonly List<Payment> _payments = [];

    public Guid BranchId { get; private set; }

    public Guid? CustomerId { get; private set; }

    public string OrderNumber { get; private set; } = string.Empty;

    public string? CreationKey { get; private set; }

    public string? CreationFingerprint { get; private set; }

    public OrderType Type { get; private set; }

    public OrderStatus Status { get; private set; }

    public Money TotalAmount { get; private set; }

    public IReadOnlyCollection<OrderLine> Lines =>
        _lines.AsReadOnly();

    public IReadOnlyCollection<Payment> Payments =>
        _payments.AsReadOnly();

    private Order()
    {
    }

    public static Order Create(
        Guid branchId,
        string orderNumber,
        OrderType type,
        string currency,
        Guid? customerId = null,
        string? creationKey = null,
        string? creationFingerprint = null)
    {
        if (branchId == Guid.Empty)
            throw new ArgumentException(
                "Branch ID is required.");

        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException(
                "Order number is required.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException(
                "Currency is required.");

        return new Order
        {
            BranchId = branchId,
            CreationKey = creationKey,
            CreationFingerprint = creationFingerprint,
            CustomerId = customerId,
            OrderNumber = orderNumber.Trim(),
            Type = type,
            Status = OrderStatus.Draft,
            TotalAmount = new Money(
                0,
                currency.Trim().ToUpperInvariant())
        };
    }

    public void AddLine(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        EnsureEditable();

        if (line.OrderId != Id)
        {
            throw new InvalidOperationException(
                "Order line belongs to another order.");
        }

        if (_lines.Any(x => x.Id == line.Id))
        {
            throw new InvalidOperationException(
                "Order line already exists.");
        }

        if (!string.Equals(
                line.TotalPrice.Currency,
                TotalAmount.Currency,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Order line currency does not match order currency.");
        }

        _lines.Add(line);

        RecalculateTotal();
    }

    public void RemoveLine(Guid orderLineId)
    {
        EnsureEditable();

        var line = _lines.FirstOrDefault(
            x => x.Id == orderLineId);

        if (line is null)
        {
            throw new InvalidOperationException(
                "Order line was not found.");
        }

        _lines.Remove(line);

        RecalculateTotal();
    }

    public void SubmitForPayment()
    {
        EnsureStatus(OrderStatus.Draft);

        if (_lines.Count == 0)
        {
            throw new InvalidOperationException(
                "Order must contain at least one line.");
        }

        Status = OrderStatus.PendingPayment;
    }

    public void MarkPartiallyPaid()
    {
        if (Status != OrderStatus.PendingPayment &&
            Status != OrderStatus.PartiallyPaid)
        {
            throw new InvalidOperationException(
                $"Order cannot be partially paid from status {Status}.");
        }

        Status = OrderStatus.PartiallyPaid;
        Touch();
    }

    public void MarkPaid()
    {
        if (Status != OrderStatus.PendingPayment &&
            Status != OrderStatus.PartiallyPaid)
        {
            throw new InvalidOperationException(
                $"Order cannot be paid from status {Status}.");
        }

        Status = OrderStatus.Paid;
    }

    public void Accept()
    {
        EnsureStatus(OrderStatus.Paid);

        Status = OrderStatus.Accepted;
    }

    public void StartPreparing()
    {
        EnsureStatus(OrderStatus.Accepted);

        Status = OrderStatus.Preparing;
    }

    public void MarkAwaitingAssembly()
    {
        if (Status != OrderStatus.Accepted &&
            Status != OrderStatus.Preparing)
        {
            throw new InvalidOperationException(
                $"Order cannot move to awaiting assembly from status {Status}.");
        }

        Status = OrderStatus.AwaitingAssembly;
    }

    public void MarkReady()
    {
        EnsureStatus(OrderStatus.AwaitingAssembly);

        Status = OrderStatus.Ready;
    }

    public void MarkAwaitingPickup()
    {
        EnsureStatus(OrderStatus.Ready);

        if (Type != OrderType.Pickup)
        {
            throw new InvalidOperationException(
                "Only pickup orders can await pickup.");
        }

        Status = OrderStatus.AwaitingPickup;
    }

    public void MarkOutForDelivery()
    {
        EnsureStatus(OrderStatus.Ready);

        if (Type != OrderType.Delivery)
        {
            throw new InvalidOperationException(
                "Only delivery orders can be sent for delivery.");
        }

        Status = OrderStatus.OutForDelivery;
    }

    public void Complete()
    {
        var canComplete =
            Type == OrderType.DineIn &&
            Status == OrderStatus.Ready;

        canComplete |=
            Type == OrderType.Pickup &&
            Status == OrderStatus.AwaitingPickup;

        canComplete |=
            Type == OrderType.Delivery &&
            Status == OrderStatus.OutForDelivery;

        if (!canComplete)
        {
            throw new InvalidOperationException(
                $"Order cannot be completed from status {Status}.");
        }

        Status = OrderStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Completed ||
            Status == OrderStatus.Cancelled ||
            Status == OrderStatus.Refunded)
        {
            throw new InvalidOperationException(
                $"Order cannot be cancelled from status {Status}.");
        }

        Status = OrderStatus.Cancelled;
    }

    public void MarkRefundPending()
    {
        if (Status != OrderStatus.Cancelled &&
            Status != OrderStatus.Completed)
        {
            throw new InvalidOperationException(
                $"Refund cannot be started from status {Status}.");
        }

        Status = OrderStatus.RefundPending;
    }

    public void MarkPartiallyRefunded()
    {
        if (Status != OrderStatus.RefundPending &&
            Status != OrderStatus.PartiallyRefunded)
        {
            throw new InvalidOperationException(
                $"Order cannot be partially refunded from status {Status}.");
        }

        Status = OrderStatus.PartiallyRefunded;
        Touch();
    }

    public void MarkRefunded()
    {
        if (Status != OrderStatus.RefundPending &&
            Status != OrderStatus.PartiallyRefunded)
        {
            throw new InvalidOperationException(
                $"Order cannot be refunded from status {Status}.");
        }

        Status = OrderStatus.Refunded;
    }

    private void RecalculateTotal()
    {
        var total = _lines.Sum(
            x => x.TotalPrice.Amount);

        TotalAmount = new Money(
            total,
            TotalAmount.Currency);
    }

    private void EnsureEditable()
    {
        if (Status != OrderStatus.Draft)
        {
            throw new InvalidOperationException(
                "Order can only be edited while it is in Draft status.");
        }
    }

    private void EnsureStatus(
        OrderStatus requiredStatus)
    {
        if (Status != requiredStatus)
        {
            throw new InvalidOperationException(
                $"Expected order status {requiredStatus}, but current status is {Status}.");
        }
    }
}