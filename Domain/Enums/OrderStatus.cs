namespace QSR.Domain.Enums;

public enum OrderStatus
{
    Draft = 1,
    PendingPayment = 2,
    PartiallyPaid = 3,
    Paid = 4,
    Accepted = 5,
    Preparing = 6,
    AwaitingAssembly = 7,
    Ready = 8,
    AwaitingPickup = 9,
    OutForDelivery = 10,
    Completed = 11,
    Cancelled = 12,
    RefundPending = 13,
    PartiallyRefunded = 14,
    Refunded = 15
}