using QSR.Domain.Enums;

namespace QSR.Application.Features.Orders.GetOrdersByBranch;

public sealed record GetOrdersByBranchResult(
    Guid Id,
    string OrderNumber,
    OrderType Type,
    OrderStatus Status,
    decimal TotalAmount,
    string Currency,
    DateTimeOffset CreatedAt);