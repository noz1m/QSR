using QSR.Domain.Enums;

namespace QSR.Application.Features.Orders.CreateOrder;

public sealed record CreateOrderResult(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    decimal TotalAmount,
    string Currency);