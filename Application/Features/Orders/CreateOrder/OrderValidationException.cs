namespace QSR.Application.Features.Orders.CreateOrder;

public sealed class OrderValidationException(string message) : Exception(message);
