using QSR.Domain.Enums;

namespace QSR.Application.Features.Orders.GetOrderById;

public sealed record GetOrderByIdResult(
    Guid Id,
    Guid BranchId,
    Guid? CustomerId,
    string OrderNumber,
    OrderType Type,
    OrderStatus Status,
    decimal TotalAmount,
    string Currency,
    DateTimeOffset CreatedAt,
    IReadOnlyCollection<GetOrderLineResult> Lines);