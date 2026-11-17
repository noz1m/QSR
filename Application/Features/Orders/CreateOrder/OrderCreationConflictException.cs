namespace QSR.Application.Features.Orders.CreateOrder;

public sealed class OrderCreationConflictException()
    : Exception("The idempotency key was already used with a different order.");
